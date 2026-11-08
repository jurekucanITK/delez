using Delez.Api.Data;
using Delez.Api.Dto;
using Delez.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Delez.Api.Services;

public enum NapakaDodajanjaStroska
{
    PlacnikNiClan,
    SamoSebe
}

public record RezultatDodajanjaStroska(StrosekDto? Strosek, NapakaDodajanjaStroska? Napaka);

public class StroskiService(DelezDbContext db)
{
    public async Task<List<StrosekDto>> SeznamAsync(int skupinaId) =>
        await db.Stroski
            .AsNoTracking()
            .Where(s => s.SkupinaId == skupinaId)
            .OrderByDescending(s => s.Datum)
            .ThenByDescending(s => s.Id)
            .Select(s => new StrosekDto(s.Id, s.Opis, s.Znesek, s.Datum, s.PlacnikId, s.Placnik.Uporabnik.Ime))
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

        var strosek = new Strosek
        {
            Opis = zahtevek.Opis!.Trim(),
            Znesek = zahtevek.Znesek!.Value,
            Datum = zahtevek.Datum!.Value,
            SkupinaId = skupinaId,
            PlacnikId = placnikId
        };

        db.Stroski.Add(strosek);
        await db.SaveChangesAsync();

        return new RezultatDodajanjaStroska(
            new StrosekDto(strosek.Id, strosek.Opis, strosek.Znesek, strosek.Datum, strosek.PlacnikId, placnikIme),
            null);
    }
}
