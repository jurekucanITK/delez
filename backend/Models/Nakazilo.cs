namespace Delez.Api.Models;

public class Nakazilo
{
    public int Id { get; set; }
    public decimal Znesek { get; set; }
    public DateOnly Datum { get; set; }

    public int SkupinaId { get; set; }
    public Skupina Skupina { get; set; } = null!;

    public int PlacnikId { get; set; }
    public Clanstvo Placnik { get; set; } = null!;

    public int PrejemnikId { get; set; }
    public Clanstvo Prejemnik { get; set; } = null!;
}
