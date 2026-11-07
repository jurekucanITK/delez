using System.ComponentModel.DataAnnotations;

namespace Delez.Api.Dto;

public class NovaSkupinaZahtevek
{
    [Required(ErrorMessage = "Ime skupine je obvezno")]
    [MaxLength(100, ErrorMessage = "Ime skupine je lahko dolgo največ 100 znakov")]
    public string? Ime { get; set; }

    [MaxLength(500, ErrorMessage = "Opis je lahko dolg največ 500 znakov")]
    public string? Opis { get; set; }
}
