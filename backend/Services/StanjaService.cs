using Delez.Api.Data;
using Delez.Api.Domain;
using Delez.Api.Dto;
using Microsoft.EntityFrameworkCore;

namespace Delez.Api.Services;

public class StanjaService(DelezDbContext db)
{
    public async Task<Dictionary<int, decimal>> IzracunajAsync(IReadOnlyCollection<int> skupinaIds)
    {
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

        var stanja = new Dictionary<int, decimal>();

        foreach (var skupinaId in skupinaIds.Distinct())
        {
            var stanjaSkupine = IzracunStanj.Izracunaj(
                clanstva.Where(c => c.SkupinaId == skupinaId).Select(c => c.Id),
                stroski.Where(s => s.SkupinaId == skupinaId),
                nakazila.Where(n => n.SkupinaId == skupinaId));

            foreach (var (clanstvoId, stanje) in stanjaSkupine)
            {
                stanja[clanstvoId] = stanje;
            }
        }

        return stanja;
    }

    public async Task<List<StanjeClanaDto>> SeznamAsync(int skupinaId)
    {
        var stanja = await IzracunajAsync([skupinaId]);

        var clani = await db.Clanstva
            .AsNoTracking()
            .Where(c => c.SkupinaId == skupinaId)
            .OrderBy(c => c.Uporabnik.Ime)
            .ThenBy(c => c.Uporabnik.Email)
            .Select(c => new { c.Id, c.Uporabnik.Ime, c.Uporabnik.Email })
            .ToListAsync();

        return clani
            .Select(c => new StanjeClanaDto(c.Id, c.Ime, c.Email, stanja.GetValueOrDefault(c.Id)))
            .ToList();
    }

    public async Task<List<PredlogNakazilaDto>> PredlogNakazilAsync(int skupinaId)
    {
        var stanja = await IzracunajAsync([skupinaId]);
        var predlog = PredlogNakazil.Izracunaj(stanja);

        var clani = await db.Clanstva
            .AsNoTracking()
            .Where(c => c.SkupinaId == skupinaId)
            .Select(c => new { c.Id, c.Uporabnik.Ime, c.Uporabnik.Email })
            .ToDictionaryAsync(c => c.Id);

        return predlog
            .Select(p => new PredlogNakazilaDto(
                p.PlacnikId,
                clani[p.PlacnikId].Ime,
                clani[p.PlacnikId].Email,
                p.PrejemnikId,
                clani[p.PrejemnikId].Ime,
                clani[p.PrejemnikId].Email,
                p.Znesek))
            .ToList();
    }
}
