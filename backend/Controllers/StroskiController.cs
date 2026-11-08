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

        return Ok(await stroski.SeznamAsync(id, rezultat.Clanstvo!.Id));
    }

    [HttpGet("{strosekId:int}")]
    [ProducesResponseType<StrosekPodrobnostiDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Podrobnosti(int id, int strosekId)
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

        var strosek = await stroski.PodrobnostiAsync(id, strosekId);
        return strosek is null ? StrosekNeObstaja() : Ok(strosek);
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
            return OdgovorZaNapako(napaka);
        }

        return StatusCode(StatusCodes.Status201Created, rezultat.Strosek);
    }

    [HttpPut("{strosekId:int}")]
    [ProducesResponseType<StrosekDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Uredi(int id, int strosekId, NovStrosekZahtevek zahtevek)
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

        var rezultat = await stroski.UrediAsync(id, dostopRezultat.Clanstvo!, strosekId, zahtevek);

        if (rezultat.Napaka is { } napaka)
        {
            return OdgovorZaNapako(napaka);
        }

        return Ok(rezultat.Strosek);
    }

    [HttpDelete("{strosekId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Izbrisi(int id, int strosekId)
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

        var napaka = await stroski.IzbrisiAsync(id, dostopRezultat.Clanstvo!, strosekId);

        return napaka is { } n ? OdgovorZaNapako(n) : NoContent();
    }

    private IActionResult StrosekNeObstaja() => Problem(
        detail: "Strošek ne obstaja",
        statusCode: StatusCodes.Status404NotFound);

    private IActionResult OdgovorZaNapako(NapakaStroska napaka)
    {
        switch (napaka)
        {
            case NapakaStroska.NeObstaja:
                return StrosekNeObstaja();
            case NapakaStroska.NimaPravic:
                return OdgovorZaDostop(StanjeDostopa.NimaPravic);
            case NapakaStroska.ClanNiVSkupini:
                ModelState.AddModelError("claniIds", "Izbrani član ni član skupine");
                break;
            case NapakaStroska.SamoSebe:
                ModelState.AddModelError("placnikId", "Kot plačnika lahko izbereš samo sebe");
                break;
            default:
                ModelState.AddModelError("placnikId", "Plačnik ni član skupine");
                break;
        }
        return ValidationProblem(ModelState);
    }
}
