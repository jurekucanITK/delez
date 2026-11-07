using System.ComponentModel.DataAnnotations;
using Delez.Api.Models;

namespace Delez.Api.Dto;

public class NovClanZahtevek
{
    [Required(ErrorMessage = "E-poštni naslov je obvezen")]
    [EmailAddress(ErrorMessage = "Vnesi veljaven e-poštni naslov")]
    [MaxLength(254, ErrorMessage = "E-poštni naslov je lahko dolg največ 254 znakov")]
    public string? Email { get; set; }

    [AllowedValues(Vloge.Skrbnik, Vloge.Clan, Vloge.Omejen, ErrorMessage = "Vloga ni veljavna")]
    public string Vloga { get; set; } = Vloge.Clan;
}
