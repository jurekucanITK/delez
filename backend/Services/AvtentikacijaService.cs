using Delez.Api.Data;
using Delez.Api.Dto;
using Delez.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Delez.Api.Services;

public record RezultatRegistracije(UporabnikDto? Uporabnik, bool EmailZeObstaja);

public class AvtentikacijaService(DelezDbContext db)
{
    public async Task<RezultatRegistracije> RegistrirajAsync(RegistracijaZahtevek zahtevek)
    {
        var ime = zahtevek.Ime!.Trim();
        var email = zahtevek.Email!.Trim().ToLowerInvariant();

        if (await db.Uporabniki.AnyAsync(u => u.Email == email))
        {
            return new RezultatRegistracije(null, true);
        }

        var uporabnik = new Uporabnik
        {
            Ime = ime,
            Email = email,
            GesloHash = BCrypt.Net.BCrypt.HashPassword(zahtevek.Geslo)
        };

        db.Uporabniki.Add(uporabnik);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return new RezultatRegistracije(null, true);
        }

        return new RezultatRegistracije(
            new UporabnikDto(uporabnik.Id, uporabnik.Ime, uporabnik.Email), false);
    }
}
