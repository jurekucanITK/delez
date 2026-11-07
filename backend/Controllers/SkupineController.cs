using Delez.Api.Dto;
using Delez.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Delez.Api.Controllers;

[ApiController]
[Route("api/skupine")]
public class SkupineController(SkupineService skupine) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<List<SkupinaPovzetekDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Seznam()
    {
        if (!PreberiUporabnikId(out var uporabnikId))
        {
            return Unauthorized();
        }

        return Ok(await skupine.SeznamAsync(uporabnikId));
    }

    [HttpPost]
    [ProducesResponseType<SkupinaDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Ustvari(NovaSkupinaZahtevek zahtevek)
    {
        if (!PreberiUporabnikId(out var uporabnikId))
        {
            return Unauthorized();
        }

        var skupina = await skupine.UstvariAsync(uporabnikId, zahtevek);
        return CreatedAtAction(nameof(Pridobi), new { id = skupina.Id }, skupina);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<SkupinaDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Pridobi(int id)
    {
        if (!PreberiUporabnikId(out var uporabnikId))
        {
            return Unauthorized();
        }

        var rezultat = await skupine.PridobiAsync(uporabnikId, id);

        return rezultat.Stanje switch
        {
            StanjeDostopa.NeObstaja => Problem(
                detail: "Skupina ne obstaja",
                statusCode: StatusCodes.Status404NotFound),
            StanjeDostopa.NiClan => Problem(
                detail: "Nisi član te skupine",
                statusCode: StatusCodes.Status403Forbidden),
            _ => Ok(rezultat.Skupina)
        };
    }

    private bool PreberiUporabnikId(out int uporabnikId) =>
        int.TryParse(User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value, out uporabnikId);
}
