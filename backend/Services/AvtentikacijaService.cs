using System.Security.Claims;
using System.Text;
using Delez.Api.Data;
using Delez.Api.Dto;
using Delez.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Delez.Api.Services;

public record RezultatRegistracije(UporabnikDto? Uporabnik, bool EmailZeObstaja);

public class AvtentikacijaService(DelezDbContext db, IConfiguration konfiguracija)
{
    public async Task<RezultatRegistracije> RegistrirajAsync(RegistracijaZahtevek zahtevek)
    {
        var ime = zahtevek.Ime!.Trim();
        var email = NormalizirajEmail(zahtevek.Email!);

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

        return new RezultatRegistracije(VUporabnikDto(uporabnik), false);
    }

    public async Task<PrijavaOdgovor?> PrijaviAsync(PrijavaZahtevek zahtevek)
    {
        var email = NormalizirajEmail(zahtevek.Email!);

        var uporabnik = await db.Uporabniki.AsNoTracking().SingleOrDefaultAsync(u => u.Email == email);

        if (uporabnik is null || !BCrypt.Net.BCrypt.Verify(zahtevek.Geslo, uporabnik.GesloHash))
        {
            return null;
        }

        return new PrijavaOdgovor(UstvariZeton(uporabnik), VUporabnikDto(uporabnik));
    }

    public async Task<UporabnikDto?> PridobiUporabnikaAsync(int id)
    {
        var uporabnik = await db.Uporabniki.AsNoTracking().SingleOrDefaultAsync(u => u.Id == id);
        return uporabnik is null ? null : VUporabnikDto(uporabnik);
    }

    private string UstvariZeton(Uporabnik uporabnik)
    {
        var jwt = konfiguracija.GetSection("Jwt");
        var kljuc = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Kljuc"]!));
        var veljavnostMinut = jwt.GetValue("VeljavnostMinut", 60);

        var opis = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, uporabnik.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, uporabnik.Email),
                new Claim(JwtRegisteredClaimNames.Name, uporabnik.Ime)
            ]),
            Issuer = jwt["Izdajatelj"],
            Audience = jwt["Obcinstvo"],
            Expires = DateTime.UtcNow.AddMinutes(veljavnostMinut),
            SigningCredentials = new SigningCredentials(kljuc, SecurityAlgorithms.HmacSha256)
        };

        return new JsonWebTokenHandler().CreateToken(opis);
    }

    public static string NormalizirajEmail(string email) => email.Trim().ToLowerInvariant();

    private static UporabnikDto VUporabnikDto(Uporabnik uporabnik) =>
        new(uporabnik.Id, uporabnik.Ime, uporabnik.Email);
}
