using Delez.Api.Dto;
using Delez.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Delez.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AvtentikacijaController(AvtentikacijaService avtentikacija) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("registracija")]
    [ProducesResponseType<UporabnikDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Registracija(RegistracijaZahtevek zahtevek)
    {
        var rezultat = await avtentikacija.RegistrirajAsync(zahtevek);

        if (rezultat.EmailZeObstaja)
        {
            ModelState.AddModelError("email", "Ta e-poštni naslov je že registriran");
            return ValidationProblem(ModelState);
        }

        return StatusCode(StatusCodes.Status201Created, rezultat.Uporabnik);
    }

    [AllowAnonymous]
    [HttpPost("prijava")]
    [ProducesResponseType<PrijavaOdgovor>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Prijava(PrijavaZahtevek zahtevek)
    {
        var odgovor = await avtentikacija.PrijaviAsync(zahtevek);

        if (odgovor is null)
        {
            return Problem(
                detail: "Napačen e-poštni naslov ali geslo",
                statusCode: StatusCodes.Status401Unauthorized);
        }

        return Ok(odgovor);
    }

    [HttpGet("jaz")]
    [ProducesResponseType<UporabnikDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Jaz()
    {
        if (!int.TryParse(User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value, out var id))
        {
            return Unauthorized();
        }

        var uporabnik = await avtentikacija.PridobiUporabnikaAsync(id);
        return uporabnik is null ? Unauthorized() : Ok(uporabnik);
    }
}
