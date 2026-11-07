using Delez.Api.Models;

namespace Delez.Api.Domain;

public static class IzracunStanj
{
    public static Dictionary<int, decimal> Izracunaj(
        IEnumerable<int> clanstvaIds,
        IEnumerable<Strosek> stroski,
        IEnumerable<Nakazilo> nakazila)
    {
        var stanja = clanstvaIds.ToDictionary(id => id, _ => 0m);

        foreach (var strosek in stroski)
        {
            Pristej(stanja, strosek.PlacnikId, strosek.Znesek);

            foreach (var delez in strosek.Delezi)
            {
                Pristej(stanja, delez.ClanstvoId, -delez.Znesek);
            }
        }

        foreach (var nakazilo in nakazila)
        {
            Pristej(stanja, nakazilo.PlacnikId, nakazilo.Znesek);
            Pristej(stanja, nakazilo.PrejemnikId, -nakazilo.Znesek);
        }

        return stanja;
    }

    private static void Pristej(Dictionary<int, decimal> stanja, int clanstvoId, decimal znesek)
    {
        stanja[clanstvoId] = stanja.GetValueOrDefault(clanstvoId) + znesek;
    }
}
