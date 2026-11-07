namespace Delez.Api.Models;

public class DelezStroska
{
    public int Id { get; set; }
    public decimal Znesek { get; set; }

    public int StrosekId { get; set; }
    public Strosek Strosek { get; set; } = null!;

    public int ClanstvoId { get; set; }
    public Clanstvo Clanstvo { get; set; } = null!;
}
