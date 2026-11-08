using Delez.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Delez.Api.Controllers;

public abstract class SkupinaControllerBase : ControllerBase
{
    protected bool PreberiUporabnikId(out int uporabnikId) =>
        int.TryParse(User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value, out uporabnikId);

    protected IActionResult OdgovorZaDostop(StanjeDostopa stanje) => stanje switch
    {
        StanjeDostopa.NeObstaja => Problem(
            detail: "Skupina ne obstaja",
            statusCode: StatusCodes.Status404NotFound),
        StanjeDostopa.NiClan => Problem(
            detail: "Nisi član te skupine",
            statusCode: StatusCodes.Status403Forbidden),
        StanjeDostopa.NimaPravic => Problem(
            detail: "Za to dejanje nimaš pravic",
            statusCode: StatusCodes.Status403Forbidden),
        _ => throw new ArgumentOutOfRangeException(nameof(stanje))
    };
}
