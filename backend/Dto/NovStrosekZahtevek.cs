using System.ComponentModel.DataAnnotations;

namespace Delez.Api.Dto;

public class NovStrosekZahtevek
{
    [Required(ErrorMessage = "Opis je obvezen")]
    [MaxLength(200, ErrorMessage = "Opis je lahko dolg največ 200 znakov")]
    public string? Opis { get; set; }

    [Required(ErrorMessage = "Znesek je obvezen")]
    [VeljavenZnesek]
    public decimal? Znesek { get; set; }

    [Required(ErrorMessage = "Datum je obvezen")]
    public DateOnly? Datum { get; set; }

    [Required(ErrorMessage = "Plačnik je obvezen")]
    public int? PlacnikId { get; set; }
}
