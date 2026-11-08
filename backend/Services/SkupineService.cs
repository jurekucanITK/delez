using Delez.Api.Data;
using Delez.Api.Dto;
using Delez.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Delez.Api.Services;

public record RezultatPridobitveSkupine(StanjeDostopa Stanje, SkupinaDto? Skupina);

public class SkupineService(DelezDbContext db, DostopService dostop, StanjaService stanjaService)
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

        return VSkupinaDto(skupina, skupina.Clanstva[0]);
    }

    public async Task<RezultatPridobitveSkupine> PridobiAsync(int uporabnikId, int skupinaId)
    {
        var rezultat = await dostop.PreveriAsync(uporabnikId, skupinaId);

        if (rezultat.Stanje != StanjeDostopa.Uspeh)
        {
            return new RezultatPridobitveSkupine(rezultat.Stanje, null);
        }

        var clanstvo = rezultat.Clanstvo!;
        return new RezultatPridobitveSkupine(StanjeDostopa.Uspeh, VSkupinaDto(clanstvo.Skupina, clanstvo));
    }

    public async Task<List<SkupinaPovzetekDto>> SeznamAsync(int uporabnikId)
    {
        var mojaClanstva = await db.Clanstva
            .AsNoTracking()
            .Where(c => c.UporabnikId == uporabnikId)
            .Include(c => c.Skupina)
            .ToListAsync();

        var stanja = await stanjaService.IzracunajAsync(mojaClanstva.Select(c => c.SkupinaId).ToList());

        return mojaClanstva
            .Select(clanstvo => new SkupinaPovzetekDto(
                clanstvo.SkupinaId,
                clanstvo.Skupina.Ime,
                clanstvo.Vloga,
                stanja[clanstvo.Id]))
            .OrderBy(s => s.Ime)
            .ToList();
    }

    private static SkupinaDto VSkupinaDto(Skupina skupina, Clanstvo clanstvo) =>
        new(skupina.Id, skupina.Ime, skupina.Opis, clanstvo.Vloga, clanstvo.Id);
}
