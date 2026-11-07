using System.ComponentModel.DataAnnotations;

namespace Delez.Api.Dto;

public class PrijavaZahtevek
{
    [Required(ErrorMessage = "E-poštni naslov je obvezen")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Geslo je obvezno")]
    public string? Geslo { get; set; }
}
