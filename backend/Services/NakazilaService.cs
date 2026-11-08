using Delez.Api.Data;
using Delez.Api.Dto;
using Delez.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Delez.Api.Services;

public enum NapakaDodajanjaNakazila
{
    SamoSebe,
    IstaOseba,
    PlacnikNiClan,
    PrejemnikNiClan
}

public record RezultatDodajanjaNakazila(NakaziloDto? Nakazilo, NapakaDodajanjaNakazila? Napaka);

public class NakazilaService(DelezDbContext db)
{
    public async Task<RezultatDodajanjaNakazila> DodajAsync(int skupinaId, Clanstvo moje, NovoNakaziloZahtevek zahtevek)
    {
        var placnikId = zahtevek.PlacnikId!.Value;
        var prejemnikId = zahtevek.PrejemnikId!.Value;

        if (moje.Vloga == Vloge.Clan && placnikId != moje.Id)
        {
            return new RezultatDodajanjaNakazila(null, NapakaDodajanjaNakazila.SamoSebe);
        }

        if (placnikId == prejemnikId)
        {
            return new RezultatDodajanjaNakazila(null, NapakaDodajanjaNakazila.IstaOseba);
        }

        var imena = await db.Clanstva
            .AsNoTracking()
            .Where(c => c.SkupinaId == skupinaId && (c.Id == placnikId || c.Id == prejemnikId))
            .ToDictionaryAsync(c => c.Id, c => c.Uporabnik.Ime);

        if (!imena.TryGetValue(placnikId, out var placnikIme))
        {
            return new RezultatDodajanjaNakazila(null, NapakaDodajanjaNakazila.PlacnikNiClan);
        }

        if (!imena.TryGetValue(prejemnikId, out var prejemnikIme))
        {
            return new RezultatDodajanjaNakazila(null, NapakaDodajanjaNakazila.PrejemnikNiClan);
        }

        var nakazilo = new Nakazilo
        {
            Znesek = zahtevek.Znesek!.Value,
            Datum = zahtevek.Datum ?? DateOnly.FromDateTime(DateTime.Today),
            SkupinaId = skupinaId,
            PlacnikId = placnikId,
            PrejemnikId = prejemnikId
        };

        db.Nakazila.Add(nakazilo);
        await db.SaveChangesAsync();

        return new RezultatDodajanjaNakazila(
            new NakaziloDto(
                nakazilo.Id,
                nakazilo.Znesek,
                nakazilo.Datum,
                placnikId,
                placnikIme,
                prejemnikId,
                prejemnikIme),
            null);
    }
}
