using Delez.Api.Data;
using Delez.Api.Domain;
using Delez.Api.Dto;
using Delez.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Delez.Api.Services;

public enum NapakaStroska
{
    PlacnikNiClan,
    SamoSebe,
    ClanNiVSkupini,
    NeObstaja,
    NimaPravic
}

public record RezultatStroska(StrosekDto? Strosek, NapakaStroska? Napaka);

public class StroskiService(DelezDbContext db)
{
    private record PreverjenZahtevek(NapakaStroska? Napaka, string PlacnikIme, List<int> Izbrani);

    public async Task<List<StrosekDto>> SeznamAsync(int skupinaId, int mojeClanstvoId) =>
        await db.Stroski
            .AsNoTracking()
            .Where(s => s.SkupinaId == skupinaId)
            .OrderByDescending(s => s.Datum)
            .ThenByDescending(s => s.Id)
            .Select(s => new StrosekDto(
                s.Id,
                s.Opis,
                s.Znesek,
                s.Datum,
                s.PlacnikId,
                s.Placnik.Uporabnik.Ime,
                s.Delezi
                    .Where(d => d.ClanstvoId == mojeClanstvoId)
                    .Select(d => (decimal?)d.Znesek)
                    .FirstOrDefault()))
            .ToListAsync();

    public async Task<StrosekPodrobnostiDto?> PodrobnostiAsync(int skupinaId, int strosekId) =>
        await db.Stroski
            .AsNoTracking()
            .Where(s => s.Id == strosekId && s.SkupinaId == skupinaId)
            .Select(s => new StrosekPodrobnostiDto(
                s.Id,
                s.Opis,
                s.Znesek,
                s.Datum,
                s.PlacnikId,
                s.Delezi.Select(d => d.ClanstvoId).OrderBy(id => id).ToList()))
            .SingleOrDefaultAsync();

    public async Task<RezultatStroska> DodajAsync(int skupinaId, Clanstvo moje, NovStrosekZahtevek zahtevek)
    {
        var preverjen = await PreveriZahtevekAsync(skupinaId, moje, zahtevek);
        if (preverjen.Napaka is { } napaka)
        {
            return new RezultatStroska(null, napaka);
        }

        var znesek = zahtevek.Znesek!.Value;
        var delezi = DelitevStroska.Enakomerno(znesek, preverjen.Izbrani);

        var strosek = new Strosek
        {
            Opis = zahtevek.Opis!.Trim(),
            Znesek = znesek,
            Datum = zahtevek.Datum!.Value,
            SkupinaId = skupinaId,
            PlacnikId = zahtevek.PlacnikId!.Value,
            Delezi = delezi
                .Select(d => new DelezStroska { ClanstvoId = d.Key, Znesek = d.Value })
                .ToList()
        };

        db.Stroski.Add(strosek);
        await db.SaveChangesAsync();

        return new RezultatStroska(VDto(strosek, preverjen.PlacnikIme, delezi, moje.Id), null);
    }

    public async Task<RezultatStroska> UrediAsync(int skupinaId, Clanstvo moje, int strosekId, NovStrosekZahtevek zahtevek)
    {
        var strosek = await db.Stroski
            .Include(s => s.Delezi)
            .SingleOrDefaultAsync(s => s.Id == strosekId && s.SkupinaId == skupinaId);

        if (strosek is null)
        {
            return new RezultatStroska(null, NapakaStroska.NeObstaja);
        }

        if (!LahkoUpravlja(moje, strosek))
        {
            return new RezultatStroska(null, NapakaStroska.NimaPravic);
        }

        var preverjen = await PreveriZahtevekAsync(skupinaId, moje, zahtevek);
        if (preverjen.Napaka is { } napaka)
        {
            return new RezultatStroska(null, napaka);
        }

        var znesek = zahtevek.Znesek!.Value;
        var delezi = DelitevStroska.Enakomerno(znesek, preverjen.Izbrani);

        strosek.Opis = zahtevek.Opis!.Trim();
        strosek.Znesek = znesek;
        strosek.Datum = zahtevek.Datum!.Value;
        strosek.PlacnikId = zahtevek.PlacnikId!.Value;

        foreach (var obstojeci in strosek.Delezi.ToList())
        {
            if (delezi.TryGetValue(obstojeci.ClanstvoId, out var novZnesek))
            {
                obstojeci.Znesek = novZnesek;
            }
            else
            {
                strosek.Delezi.Remove(obstojeci);
                db.DeleziStroskov.Remove(obstojeci);
            }
        }

        var obstojeciIds = strosek.Delezi.Select(d => d.ClanstvoId).ToHashSet();
        foreach (var (clanstvoId, delez) in delezi.Where(d => !obstojeciIds.Contains(d.Key)))
        {
            strosek.Delezi.Add(new DelezStroska { ClanstvoId = clanstvoId, Znesek = delez });
        }

        await db.SaveChangesAsync();

        return new RezultatStroska(VDto(strosek, preverjen.PlacnikIme, delezi, moje.Id), null);
    }

    public async Task<NapakaStroska?> IzbrisiAsync(int skupinaId, Clanstvo moje, int strosekId)
    {
        var strosek = await db.Stroski
            .SingleOrDefaultAsync(s => s.Id == strosekId && s.SkupinaId == skupinaId);

        if (strosek is null)
        {
            return NapakaStroska.NeObstaja;
        }

        if (!LahkoUpravlja(moje, strosek))
        {
            return NapakaStroska.NimaPravic;
        }

        db.Stroski.Remove(strosek);
        await db.SaveChangesAsync();

        return null;
    }

    private static bool LahkoUpravlja(Clanstvo moje, Strosek strosek) =>
        moje.Vloga == Vloge.Skrbnik || (moje.Vloga == Vloge.Clan && strosek.PlacnikId == moje.Id);

    private async Task<PreverjenZahtevek> PreveriZahtevekAsync(int skupinaId, Clanstvo moje, NovStrosekZahtevek zahtevek)
    {
        var placnikId = zahtevek.PlacnikId!.Value;

        if (moje.Vloga == Vloge.Clan && placnikId != moje.Id)
        {
            return new PreverjenZahtevek(NapakaStroska.SamoSebe, string.Empty, []);
        }

        var placnikIme = await db.Clanstva
            .AsNoTracking()
            .Where(c => c.Id == placnikId && c.SkupinaId == skupinaId)
            .Select(c => c.Uporabnik.Ime)
            .SingleOrDefaultAsync();

        if (placnikIme is null)
        {
            return new PreverjenZahtevek(NapakaStroska.PlacnikNiClan, string.Empty, []);
        }

        var claniSkupine = await db.Clanstva
            .AsNoTracking()
            .Where(c => c.SkupinaId == skupinaId)
            .Select(c => c.Id)
            .ToListAsync();

        var izbrani = zahtevek.ClaniIds!.Distinct().ToList();

        if (izbrani.Except(claniSkupine).Any())
        {
            return new PreverjenZahtevek(NapakaStroska.ClanNiVSkupini, string.Empty, []);
        }

        return new PreverjenZahtevek(null, placnikIme, izbrani);
    }

    private static StrosekDto VDto(Strosek strosek, string placnikIme, Dictionary<int, decimal> delezi, int mojeClanstvoId) =>
        new(
            strosek.Id,
            strosek.Opis,
            strosek.Znesek,
            strosek.Datum,
            strosek.PlacnikId,
            placnikIme,
            delezi.TryGetValue(mojeClanstvoId, out var mojDelez) ? mojDelez : null);
}
