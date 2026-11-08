using System.ComponentModel.DataAnnotations;

namespace Delez.Api.Dto;

public class NovoNakaziloZahtevek
{
    [Required(ErrorMessage = "Znesek je obvezen")]
    [VeljavenZnesek]
    public decimal? Znesek { get; set; }

    public DateOnly? Datum { get; set; }

    [Required(ErrorMessage = "Plačnik je obvezen")]
    public int? PlacnikId { get; set; }

    [Required(ErrorMessage = "Prejemnik je obvezen")]
    public int? PrejemnikId { get; set; }
}
