using Delez.Api.Data;
using Delez.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Delez.Api.Services;

public enum StanjeDostopa
{
    Uspeh,
    NeObstaja,
    NiClan,
    NimaPravic
}

public record RezultatDostopa(StanjeDostopa Stanje, Clanstvo? Clanstvo);

public class DostopService(DelezDbContext db)
{
    public async Task<RezultatDostopa> PreveriAsync(int uporabnikId, int skupinaId, params string[] dovoljeneVloge)
    {
        var clanstvo = await db.Clanstva
            .AsNoTracking()
            .Include(c => c.Skupina)
            .SingleOrDefaultAsync(c => c.SkupinaId == skupinaId && c.UporabnikId == uporabnikId);

        if (clanstvo is null)
        {
            var obstaja = await db.Skupine.AnyAsync(s => s.Id == skupinaId);
            return new RezultatDostopa(obstaja ? StanjeDostopa.NiClan : StanjeDostopa.NeObstaja, null);
        }

        if (dovoljeneVloge.Length > 0 && !dovoljeneVloge.Contains(clanstvo.Vloga))
        {
            return new RezultatDostopa(StanjeDostopa.NimaPravic, clanstvo);
        }

        return new RezultatDostopa(StanjeDostopa.Uspeh, clanstvo);
    }
}
