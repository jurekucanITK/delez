using Delez.Api.Dto;
using Delez.Api.Models;
using Delez.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Delez.Api.Controllers;

[ApiController]
[Route("api/skupine/{id:int}/nakazila")]
public class NakazilaController(DostopService dostop, NakazilaService nakazila) : SkupinaControllerBase
{
    [HttpPost]
    [ProducesResponseType<NakaziloDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Dodaj(int id, NovoNakaziloZahtevek zahtevek)
    {
        if (!PreberiUporabnikId(out var uporabnikId))
        {
            return Unauthorized();
        }

        var dostopRezultat = await dostop.PreveriAsync(uporabnikId, id, Vloge.Skrbnik, Vloge.Clan);
        if (dostopRezultat.Stanje != StanjeDostopa.Uspeh)
        {
            return OdgovorZaDostop(dostopRezultat.Stanje);
        }

        var rezultat = await nakazila.DodajAsync(id, dostopRezultat.Clanstvo!, zahtevek);

        if (rezultat.Napaka is { } napaka)
        {
            var (polje, sporocilo) = napaka switch
            {
                NapakaDodajanjaNakazila.SamoSebe => ("placnikId", "Kot plačnika lahko izbereš samo sebe"),
                NapakaDodajanjaNakazila.PlacnikNiClan => ("placnikId", "Plačnik ni član skupine"),
                NapakaDodajanjaNakazila.IstaOseba => ("prejemnikId", "Plačnik in prejemnik ne smeta biti ista oseba"),
                _ => ("prejemnikId", "Prejemnik ni član skupine")
            };
            ModelState.AddModelError(polje, sporocilo);
            return ValidationProblem(ModelState);
        }

        return StatusCode(StatusCodes.Status201Created, rezultat.Nakazilo);
    }
}
