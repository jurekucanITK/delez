namespace Delez.Api.Models;

public class Strosek
{
    public int Id { get; set; }
    public string Opis { get; set; } = string.Empty;
    public decimal Znesek { get; set; }
    public DateOnly Datum { get; set; }

    public int SkupinaId { get; set; }
    public Skupina Skupina { get; set; } = null!;

    public int PlacnikId { get; set; }
    public Clanstvo Placnik { get; set; } = null!;

    public List<DelezStroska> Delezi { get; set; } = [];
}
