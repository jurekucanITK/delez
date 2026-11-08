namespace Delez.Api.Domain;

public static class DelitevStroska
{
    public static Dictionary<int, decimal> Enakomerno(decimal znesek, IEnumerable<int> clanstvaIds)
    {
        var ids = clanstvaIds.Distinct().Order().ToList();

        if (ids.Count == 0)
        {
            throw new ArgumentException("Seznam članov je prazen.", nameof(clanstvaIds));
        }

        var centi = znesek * 100m;
        var osnova = Math.Floor(centi / ids.Count);
        var ostanek = centi - osnova * ids.Count;

        var delezi = ids.ToDictionary(id => id, _ => osnova / 100m);
        delezi[ids[0]] = (osnova + ostanek) / 100m;

        return delezi;
    }
}
