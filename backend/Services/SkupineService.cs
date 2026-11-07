using Delez.Api.Data;
using Delez.Api.Dto;
using Delez.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Delez.Api.Services;

public enum StanjeDostopa
{
    Uspeh,
    NeObstaja,
    NiClan
}

public record RezultatPridobitveSkupine(StanjeDostopa Stanje, SkupinaDto? Skupina);

public class SkupineService(DelezDbContext db)
{
    public async Task<SkupinaDto> UstvariAsync(int uporabnikId, NovaSkupinaZahtevek zahtevek)
    {
        var opis = zahtevek.Opis?.Trim();

        var skupina = new Skupina
        {
            Ime = zahtevek.Ime!.Trim(),
            Opis = string.IsNullOrEmpty(opis) ? null : opis,
            Clanstva = [new Clanstvo { UporabnikId = uporabnikId, Vloga = Vloge.Skrbnik }]
        };

        db.Skupine.Add(skupina);
        await db.SaveChangesAsync();

        return VSkupinaDto(skupina);
    }

    public async Task<RezultatPridobitveSkupine> PridobiAsync(int uporabnikId, int skupinaId)
    {
        var skupina = await db.Skupine.AsNoTracking().SingleOrDefaultAsync(s => s.Id == skupinaId);

        if (skupina is null)
        {
            return new RezultatPridobitveSkupine(StanjeDostopa.NeObstaja, null);
        }

        var jeClan = await db.Clanstva.AnyAsync(c => c.SkupinaId == skupinaId && c.UporabnikId == uporabnikId);

        if (!jeClan)
        {
            return new RezultatPridobitveSkupine(StanjeDostopa.NiClan, null);
        }

        return new RezultatPridobitveSkupine(StanjeDostopa.Uspeh, VSkupinaDto(skupina));
    }

    private static SkupinaDto VSkupinaDto(Skupina skupina) =>
        new(skupina.Id, skupina.Ime, skupina.Opis);
}
