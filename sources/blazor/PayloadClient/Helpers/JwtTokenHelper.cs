using System.IdentityModel.Tokens.Jwt;

namespace PayloadClient.Helpers;

public static class JwtTokenHelper
{
    public static string? GetUserIdFromToken(string? token)
    {
        if (string.IsNullOrEmpty(token))
            return null;

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        return jwtToken.Claims.FirstOrDefault(claim => claim.Type == "id")?.Value;
    }
} 