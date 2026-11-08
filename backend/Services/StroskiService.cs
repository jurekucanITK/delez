using Delez.Api.Data;
using Delez.Api.Domain;
using Delez.Api.Dto;
using Delez.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Delez.Api.Services;

public enum NapakaDodajanjaStroska
{
    PlacnikNiClan,
    SamoSebe,
    ClanNiVSkupini
}

public record RezultatDodajanjaStroska(StrosekDto? Strosek, NapakaDodajanjaStroska? Napaka);

public class StroskiService(DelezDbContext db)
{
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

    public async Task<RezultatDodajanjaStroska> DodajAsync(int skupinaId, Clanstvo moje, NovStrosekZahtevek zahtevek)
    {
        var placnikId = zahtevek.PlacnikId!.Value;

        if (moje.Vloga == Vloge.Clan && placnikId != moje.Id)
        {
            return new RezultatDodajanjaStroska(null, NapakaDodajanjaStroska.SamoSebe);
        }

        var placnikIme = await db.Clanstva
            .AsNoTracking()
            .Where(c => c.Id == placnikId && c.SkupinaId == skupinaId)
            .Select(c => c.Uporabnik.Ime)
            .SingleOrDefaultAsync();

        if (placnikIme is null)
        {
            return new RezultatDodajanjaStroska(null, NapakaDodajanjaStroska.PlacnikNiClan);
        }

        var claniSkupine = await db.Clanstva
            .AsNoTracking()
            .Where(c => c.SkupinaId == skupinaId)
            .Select(c => c.Id)
            .ToListAsync();

        var izbrani = zahtevek.ClaniIds!.Distinct().ToList();

        if (izbrani.Except(claniSkupine).Any())
        {
            return new RezultatDodajanjaStroska(null, NapakaDodajanjaStroska.ClanNiVSkupini);
        }

        var znesek = zahtevek.Znesek!.Value;
        var delezi = DelitevStroska.Enakomerno(znesek, izbrani);

        var strosek = new Strosek
        {
            Opis = zahtevek.Opis!.Trim(),
            Znesek = znesek,
            Datum = zahtevek.Datum!.Value,
            SkupinaId = skupinaId,
            PlacnikId = placnikId,
            Delezi = delezi
                .Select(d => new DelezStroska { ClanstvoId = d.Key, Znesek = d.Value })
                .ToList()
        };

        db.Stroski.Add(strosek);
        await db.SaveChangesAsync();

        return new RezultatDodajanjaStroska(
            new StrosekDto(
                strosek.Id,
                strosek.Opis,
                strosek.Znesek,
                strosek.Datum,
                strosek.PlacnikId,
                placnikIme,
                delezi.TryGetValue(moje.Id, out var mojDelez) ? mojDelez : null),
            null);
    }
}
