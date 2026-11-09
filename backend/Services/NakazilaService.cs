using Delez.Api.Data;
using Delez.Api.Dto;
using Delez.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Delez.Api.Services;

public enum NapakaNakazila
{
    SamoSebe,
    IstaOseba,
    PlacnikNiClan,
    PrejemnikNiClan,
    NeObstaja,
    NimaPravic
}

public record RezultatNakazila(NakaziloDto? Nakazilo, NapakaNakazila? Napaka);

public class NakazilaService(DelezDbContext db)
{
    private record PreverjenZahtevek(NapakaNakazila? Napaka, string PlacnikIme, string PrejemnikIme);

    public async Task<List<NakaziloDto>> SeznamAsync(int skupinaId) =>
        await db.Nakazila
            .AsNoTracking()
            .Where(n => n.SkupinaId == skupinaId)
            .OrderByDescending(n => n.Datum)
            .ThenByDescending(n => n.Id)
            .Select(n => new NakaziloDto(
                n.Id,
                n.Znesek,
                n.Datum,
                n.PlacnikId,
                n.Placnik.Uporabnik.Ime,
                n.PrejemnikId,
                n.Prejemnik.Uporabnik.Ime))
            .ToListAsync();

    public async Task<RezultatNakazila> DodajAsync(int skupinaId, Clanstvo moje, NovoNakaziloZahtevek zahtevek)
    {
        var preverjen = await PreveriZahtevekAsync(skupinaId, moje, zahtevek);
        if (preverjen.Napaka is { } napaka)
        {
            return new RezultatNakazila(null, napaka);
        }

        var nakazilo = new Nakazilo
        {
            Znesek = zahtevek.Znesek!.Value,
            Datum = zahtevek.Datum ?? DateOnly.FromDateTime(DateTime.Today),
            SkupinaId = skupinaId,
            PlacnikId = zahtevek.PlacnikId!.Value,
            PrejemnikId = zahtevek.PrejemnikId!.Value
        };

        db.Nakazila.Add(nakazilo);
        await db.SaveChangesAsync();

        return new RezultatNakazila(VDto(nakazilo, preverjen), null);
    }

    public async Task<RezultatNakazila> UrediAsync(int skupinaId, Clanstvo moje, int nakaziloId, NovoNakaziloZahtevek zahtevek)
    {
        var nakazilo = await db.Nakazila
            .SingleOrDefaultAsync(n => n.Id == nakaziloId && n.SkupinaId == skupinaId);

        if (nakazilo is null)
        {
            return new RezultatNakazila(null, NapakaNakazila.NeObstaja);
        }

        if (!LahkoUpravlja(moje, nakazilo))
        {
            return new RezultatNakazila(null, NapakaNakazila.NimaPravic);
        }

        var preverjen = await PreveriZahtevekAsync(skupinaId, moje, zahtevek);
        if (preverjen.Napaka is { } napaka)
        {
            return new RezultatNakazila(null, napaka);
        }

        nakazilo.Znesek = zahtevek.Znesek!.Value;
        nakazilo.Datum = zahtevek.Datum ?? nakazilo.Datum;
        nakazilo.PlacnikId = zahtevek.PlacnikId!.Value;
        nakazilo.PrejemnikId = zahtevek.PrejemnikId!.Value;

        await db.SaveChangesAsync();

        return new RezultatNakazila(VDto(nakazilo, preverjen), null);
    }

    public async Task<NapakaNakazila?> IzbrisiAsync(int skupinaId, Clanstvo moje, int nakaziloId)
    {
        var nakazilo = await db.Nakazila
            .SingleOrDefaultAsync(n => n.Id == nakaziloId && n.SkupinaId == skupinaId);

        if (nakazilo is null)
        {
            return NapakaNakazila.NeObstaja;
        }

        if (!LahkoUpravlja(moje, nakazilo))
        {
            return NapakaNakazila.NimaPravic;
        }

        db.Nakazila.Remove(nakazilo);
        await db.SaveChangesAsync();

        return null;
    }

    private static bool LahkoUpravlja(Clanstvo moje, Nakazilo nakazilo) =>
        moje.Vloga == Vloge.Skrbnik || (moje.Vloga == Vloge.Clan && nakazilo.PlacnikId == moje.Id);

    private async Task<PreverjenZahtevek> PreveriZahtevekAsync(int skupinaId, Clanstvo moje, NovoNakaziloZahtevek zahtevek)
    {
        var placnikId = zahtevek.PlacnikId!.Value;
        var prejemnikId = zahtevek.PrejemnikId!.Value;

        if (moje.Vloga == Vloge.Clan && placnikId != moje.Id)
        {
            return new PreverjenZahtevek(NapakaNakazila.SamoSebe, string.Empty, string.Empty);
        }

        if (placnikId == prejemnikId)
        {
            return new PreverjenZahtevek(NapakaNakazila.IstaOseba, string.Empty, string.Empty);
        }

        var imena = await db.Clanstva
            .AsNoTracking()
            .Where(c => c.SkupinaId == skupinaId && (c.Id == placnikId || c.Id == prejemnikId))
            .Select(c => new { c.Id, c.Uporabnik.Ime })
            .ToDictionaryAsync(c => c.Id, c => c.Ime);

        if (!imena.TryGetValue(placnikId, out var placnikIme))
        {
            return new PreverjenZahtevek(NapakaNakazila.PlacnikNiClan, string.Empty, string.Empty);
        }

        if (!imena.TryGetValue(prejemnikId, out var prejemnikIme))
        {
            return new PreverjenZahtevek(NapakaNakazila.PrejemnikNiClan, string.Empty, string.Empty);
        }

        return new PreverjenZahtevek(null, placnikIme, prejemnikIme);
    }

    private static NakaziloDto VDto(Nakazilo nakazilo, PreverjenZahtevek preverjen) =>
        new(
            nakazilo.Id,
            nakazilo.Znesek,
            nakazilo.Datum,
            nakazilo.PlacnikId,
            preverjen.PlacnikIme,
            nakazilo.PrejemnikId,
            preverjen.PrejemnikIme);
}
