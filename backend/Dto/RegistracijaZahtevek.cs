using System.ComponentModel.DataAnnotations;

namespace Delez.Api.Dto;

public class RegistracijaZahtevek
{
    [Required(ErrorMessage = "Ime je obvezno")]
    [MaxLength(100, ErrorMessage = "Ime je lahko dolgo največ 100 znakov")]
    public string? Ime { get; set; }

    [Required(ErrorMessage = "E-poštni naslov je obvezen")]
    [EmailAddress(ErrorMessage = "Vnesi veljaven e-poštni naslov")]
    [MaxLength(254, ErrorMessage = "E-poštni naslov je lahko dolg največ 254 znakov")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Geslo je obvezno")]
    [MinLength(8, ErrorMessage = "Geslo mora imeti vsaj 8 znakov.")]
    public string? Geslo { get; set; }
}
