using Delez.Api.Dto;
using Delez.Api.Models;
using Delez.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Delez.Api.Controllers;

[ApiController]
[Route("api/skupine/{id:int}/clani")]
public class ClaniController(DostopService dostop, ClaniService clani) : SkupinaControllerBase
{
    [HttpGet]
    [ProducesResponseType<List<ClanDto>>(StatusCodes.Status200OK)]
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

        return Ok(await clani.SeznamAsync(id));
    }

    [HttpPost]
    [ProducesResponseType<ClanDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Dodaj(int id, NovClanZahtevek zahtevek)
    {
        if (!PreberiUporabnikId(out var uporabnikId))
        {
            return Unauthorized();
        }

        var dostopRezultat = await dostop.PreveriAsync(uporabnikId, id, Vloge.Skrbnik);
        if (dostopRezultat.Stanje != StanjeDostopa.Uspeh)
        {
            return OdgovorZaDostop(dostopRezultat.Stanje);
        }

        var rezultat = await clani.DodajAsync(id, zahtevek);

        if (rezultat.Napaka is { } napaka)
        {
            ModelState.AddModelError("email", napaka switch
            {
                NapakaDodajanjaClana.NiRegistriran => "Uporabnik s tem naslovom ni registriran",
                _ => "Uporabnik je že član skupine"
            });
            return ValidationProblem(ModelState);
        }

        return StatusCode(StatusCodes.Status201Created, rezultat.Clan);
    }
}
