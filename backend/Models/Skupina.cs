namespace Delez.Api.Models;

public class Skupina
{
    public int Id { get; set; }
    public string Ime { get; set; } = string.Empty;
    public string? Opis { get; set; }

    public List<Clanstvo> Clanstva { get; set; } = [];
    public List<Strosek> Stroski { get; set; } = [];
    public List<Nakazilo> Nakazila { get; set; } = [];
}
