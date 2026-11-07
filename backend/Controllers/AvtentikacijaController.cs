using Delez.Api.Dto;
using Delez.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Delez.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AvtentikacijaController(AvtentikacijaService avtentikacija) : ControllerBase
{
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
}
