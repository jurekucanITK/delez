using Delez.Api.Data;
using Delez.Api.Dto;
using Delez.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Delez.Api.Services;

public enum NapakaDodajanjaClana
{
    NiRegistriran,
    ZeClan
}

public record RezultatDodajanjaClana(ClanDto? Clan, NapakaDodajanjaClana? Napaka);

public class ClaniService(DelezDbContext db)
{
    public async Task<List<ClanDto>> SeznamAsync(int skupinaId) =>
        await db.Clanstva
            .AsNoTracking()
            .Where(c => c.SkupinaId == skupinaId)
            .OrderBy(c => c.Uporabnik.Ime)
            .ThenBy(c => c.Uporabnik.Email)
            .Select(c => new ClanDto(c.Id, c.Uporabnik.Ime, c.Uporabnik.Email, c.Vloga))
            .ToListAsync();

    public async Task<RezultatDodajanjaClana> DodajAsync(int skupinaId, NovClanZahtevek zahtevek)
    {
        var email = AvtentikacijaService.NormalizirajEmail(zahtevek.Email!);

        var uporabnik = await db.Uporabniki.AsNoTracking().SingleOrDefaultAsync(u => u.Email == email);

        if (uporabnik is null)
        {
            return new RezultatDodajanjaClana(null, NapakaDodajanjaClana.NiRegistriran);
        }

        if (await db.Clanstva.AnyAsync(c => c.SkupinaId == skupinaId && c.UporabnikId == uporabnik.Id))
        {
            return new RezultatDodajanjaClana(null, NapakaDodajanjaClana.ZeClan);
        }

        var clanstvo = new Clanstvo
        {
            SkupinaId = skupinaId,
            UporabnikId = uporabnik.Id,
            Vloga = zahtevek.Vloga
        };

        db.Clanstva.Add(clanstvo);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return new RezultatDodajanjaClana(null, NapakaDodajanjaClana.ZeClan);
        }

        return new RezultatDodajanjaClana(
            new ClanDto(clanstvo.Id, uporabnik.Ime, uporabnik.Email, clanstvo.Vloga),
            null);
    }
}
