using Delez.Api.Dto;
using Delez.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Delez.Api.Controllers;

[ApiController]
[Route("api/skupine/{id:int}/predlog-nakazil")]
public class PredlogNakazilController(DostopService dostop, StanjaService stanja) : SkupinaControllerBase
{
    [HttpGet]
    [ProducesResponseType<List<PredlogNakazilaDto>>(StatusCodes.Status200OK)]
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

        return Ok(await stanja.PredlogNakazilAsync(id));
    }
}
