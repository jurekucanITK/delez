namespace Delez.Api.Domain;

public record PredlaganoNakazilo(int PlacnikId, int PrejemnikId, decimal Znesek);

public static class PredlogNakazil
{
    public static List<PredlaganoNakazilo> Izracunaj(IReadOnlyDictionary<int, decimal> stanja)
    {
        var dolzniki = stanja
            .Where(s => s.Value < 0)
            .ToDictionary(s => s.Key, s => -s.Value);
        var upniki = stanja
            .Where(s => s.Value > 0)
            .ToDictionary(s => s.Key, s => s.Value);

        var predlog = new List<PredlaganoNakazilo>();

        while (dolzniki.Count > 0 && upniki.Count > 0)
        {
            var dolznik = Najvecji(dolzniki);
            var upnik = Najvecji(upniki);
            var znesek = Math.Min(dolzniki[dolznik], upniki[upnik]);

            predlog.Add(new PredlaganoNakazilo(dolznik, upnik, znesek));

            Zmanjsaj(dolzniki, dolznik, znesek);
            Zmanjsaj(upniki, upnik, znesek);
        }

        return predlog
            .OrderByDescending(p => p.Znesek)
            .ToList();
    }

    private static int Najvecji(Dictionary<int, decimal> zneski)
    {
        return zneski
            .OrderByDescending(z => z.Value)
            .ThenBy(z => z.Key)
            .First()
            .Key;
    }

    private static void Zmanjsaj(Dictionary<int, decimal> zneski, int clanstvoId, decimal znesek)
    {
        var ostanek = zneski[clanstvoId] - znesek;
        if (ostanek == 0)
        {
            zneski.Remove(clanstvoId);
        }
        else
        {
            zneski[clanstvoId] = ostanek;
        }
    }
}
