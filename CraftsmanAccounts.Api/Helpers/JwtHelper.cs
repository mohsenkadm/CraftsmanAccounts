// مساعد JWT - إنشاء رموز المصادقة ورموز التحديث للمستخدمين
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CraftsmanAccounts.Api.Helpers;

public static class JwtHelper
{
    public static string GenerateToken(int userId, string name, string role, IConfiguration configuration)
    {
        var key = configuration["Jwt:Key"] ?? "CraftsmanAccountsSuperSecretKeyThatIs256BitsLong!!";
        var issuer = configuration["Jwt:Issuer"] ?? "CraftsmanAccounts";
        var audience = configuration["Jwt:Audience"] ?? "CraftsmanAccountsUsers";

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, name),
            new Claim(ClaimTypes.Role, role)
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // توليد رمز تحديث عشوائي آمن
    public static string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    // استخراج المطالبات من توكن منتهي الصلاحية
    public static ClaimsPrincipal? GetPrincipalFromExpiredToken(string token, IConfiguration configuration)
    {
        var key = configuration["Jwt:Key"] ?? "CraftsmanAccountsSuperSecretKeyThatIs256BitsLong!!";
        var validation = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"] ?? "CraftsmanAccounts",
            ValidAudience = configuration["Jwt:Audience"] ?? "CraftsmanAccountsUsers",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };

        var handler = new JwtSecurityTokenHandler();
        var principal = handler.ValidateToken(token, validation, out var securityToken);

        if (securityToken is not JwtSecurityToken jwt || !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            return null;

        return principal;
    }
}
