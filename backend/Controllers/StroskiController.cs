using Delez.Api.Dto;
using Delez.Api.Models;
using Delez.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Delez.Api.Controllers;

[ApiController]
[Route("api/skupine/{id:int}/stroski")]
public class StroskiController(DostopService dostop, StroskiService stroski) : SkupinaControllerBase
{
    [HttpGet]
    [ProducesResponseType<List<StrosekDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Seznam(int id)
    {
        if (!PreberiUporabnikId(out var uporabnikId))
        {
            return Unauthorized();
        }

        var rezultat = await dostop.PreveriAsync(uporabnikId, id);
        if (rezultat.Stanje != StanjeDostopa.Uspeh)
        {
            return OdgovorZaDostop(rezultat.Stanje);
        }

        return Ok(await stroski.SeznamAsync(id));
    }

    [HttpPost]
    [ProducesResponseType<StrosekDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Dodaj(int id, NovStrosekZahtevek zahtevek)
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

        var rezultat = await stroski.DodajAsync(id, dostopRezultat.Clanstvo!, zahtevek);

        if (rezultat.Napaka is { } napaka)
        {
            ModelState.AddModelError("placnikId", napaka switch
            {
                NapakaDodajanjaStroska.SamoSebe => "Kot plačnika lahko izbereš samo sebe",
                _ => "Plačnik ni član skupine"
            });
            return ValidationProblem(ModelState);
        }

        return StatusCode(StatusCodes.Status201Created, rezultat.Strosek);
    }
}
