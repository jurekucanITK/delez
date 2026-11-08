using Delez.Api.Data;
using Delez.Api.Domain;
using Delez.Api.Dto;
using Delez.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Delez.Api.Services;

public record RezultatPridobitveSkupine(StanjeDostopa Stanje, SkupinaDto? Skupina);

public class SkupineService(DelezDbContext db, DostopService dostop)
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

        var skupinaIds = mojaClanstva.Select(c => c.SkupinaId).ToList();

        var clanstva = await db.Clanstva
            .AsNoTracking()
            .Where(c => skupinaIds.Contains(c.SkupinaId))
            .Select(c => new { c.Id, c.SkupinaId })
            .ToListAsync();

        var stroski = await db.Stroski
            .AsNoTracking()
            .Where(s => skupinaIds.Contains(s.SkupinaId))
            .Include(s => s.Delezi)
            .ToListAsync();

        var nakazila = await db.Nakazila
            .AsNoTracking()
            .Where(n => skupinaIds.Contains(n.SkupinaId))
            .ToListAsync();

        return mojaClanstva
            .Select(clanstvo =>
            {
                var stanja = IzracunStanj.Izracunaj(
                    clanstva.Where(c => c.SkupinaId == clanstvo.SkupinaId).Select(c => c.Id),
                    stroski.Where(s => s.SkupinaId == clanstvo.SkupinaId),
                    nakazila.Where(n => n.SkupinaId == clanstvo.SkupinaId));

                return new SkupinaPovzetekDto(
                    clanstvo.SkupinaId,
                    clanstvo.Skupina.Ime,
                    clanstvo.Vloga,
                    stanja[clanstvo.Id]);
            })
            .OrderBy(s => s.Ime)
            .ToList();
    }

    private static SkupinaDto VSkupinaDto(Skupina skupina, Clanstvo clanstvo) =>
        new(skupina.Id, skupina.Ime, skupina.Opis, clanstvo.Vloga, clanstvo.Id);
}
