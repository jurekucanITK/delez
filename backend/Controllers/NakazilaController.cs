using Delez.Api.Dto;
using Delez.Api.Models;
using Delez.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Delez.Api.Controllers;

[ApiController]
[Route("api/skupine/{id:int}/nakazila")]
public class NakazilaController(DostopService dostop, NakazilaService nakazila) : SkupinaControllerBase
{
    [HttpGet]
    [ProducesResponseType<List<NakaziloDto>>(StatusCodes.Status200OK)]
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

        return Ok(await nakazila.SeznamAsync(id));
    }

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
            return OdgovorZaNapako(napaka);
        }

        return StatusCode(StatusCodes.Status201Created, rezultat.Nakazilo);
    }

    [HttpPut("{nakaziloId:int}")]
    [ProducesResponseType<NakaziloDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Uredi(int id, int nakaziloId, NovoNakaziloZahtevek zahtevek)
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

        var rezultat = await nakazila.UrediAsync(id, dostopRezultat.Clanstvo!, nakaziloId, zahtevek);

        if (rezultat.Napaka is { } napaka)
        {
            return OdgovorZaNapako(napaka);
        }

        return Ok(rezultat.Nakazilo);
    }

    [HttpDelete("{nakaziloId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Izbrisi(int id, int nakaziloId)
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

        var napaka = await nakazila.IzbrisiAsync(id, dostopRezultat.Clanstvo!, nakaziloId);

        return napaka is { } n ? OdgovorZaNapako(n) : NoContent();
    }

    private IActionResult OdgovorZaNapako(NapakaNakazila napaka)
    {
        if (napaka == NapakaNakazila.NeObstaja)
        {
            return Problem(detail: "Nakazilo ne obstaja", statusCode: StatusCodes.Status404NotFound);
        }

        if (napaka == NapakaNakazila.NimaPravic)
        {
            return OdgovorZaDostop(StanjeDostopa.NimaPravic);
        }

        var (polje, sporocilo) = napaka switch
        {
            NapakaNakazila.SamoSebe => ("placnikId", "Kot plačnika lahko izbereš samo sebe"),
            NapakaNakazila.PlacnikNiClan => ("placnikId", "Plačnik ni član skupine"),
            NapakaNakazila.IstaOseba => ("prejemnikId", "Plačnik in prejemnik ne smeta biti ista oseba"),
            _ => ("prejemnikId", "Prejemnik ni član skupine")
        };
        ModelState.AddModelError(polje, sporocilo);
        return ValidationProblem(ModelState);
    }
}
