namespace Delez.Api.Models;

public class Clanstvo
{
    public int Id { get; set; }
    public string Vloga { get; set; } = Vloge.Clan;

    public int UporabnikId { get; set; }
    public Uporabnik Uporabnik { get; set; } = null!;

    public int SkupinaId { get; set; }
    public Skupina Skupina { get; set; } = null!;
}
