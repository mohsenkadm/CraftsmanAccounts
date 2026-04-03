// متحكم مصادقة المستخدمين - تسجيل الدخول والتسجيل وتحديث التوكن
using CraftsmanAccounts.Api.Helpers;
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Application.Interfaces;
using CraftsmanAccounts.Domain.Entities;
using CraftsmanAccounts.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CraftsmanAccounts.Api.Controllers;

public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _config;
    private readonly IUnitOfWork _uow;

    public AuthController(IAuthService authService, IConfiguration config, IUnitOfWork uow)
    {
        _authService = authService;
        _config = config;
        _uow = uow;
    }

    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
    {
        var result = await _authService.SendOtpAsync(request.PhoneNumber);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
    {
        var result = await _authService.VerifyOtpAsync(request.PhoneNumber, request.OtpCode);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        if (!result.Success) return BadRequest(result);
        var token = JwtHelper.GenerateToken(result.Data!.UserId, result.Data.FullName, "User", _config);
        var refreshToken = JwtHelper.GenerateRefreshToken();

        // حفظ رمز التحديث
        await SaveRefreshTokenAsync(result.Data.UserId, refreshToken);

        return Ok(new { result.Success, result.Message, Data = result.Data with { Token = token, RefreshToken = refreshToken } });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (!result.Success) return BadRequest(result);
        var token = JwtHelper.GenerateToken(result.Data!.UserId, result.Data.FullName, "User", _config);
        var refreshToken = JwtHelper.GenerateRefreshToken();

        // حفظ رمز التحديث
        await SaveRefreshTokenAsync(result.Data.UserId, refreshToken);

        return Ok(new { result.Success, result.Message, Data = result.Data with { Token = token, RefreshToken = refreshToken } });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var result = await _authService.ForgotPasswordAsync(request.PhoneNumber);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var result = await _authService.ResetPasswordAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // تحديث التوكن باستخدام رمز التحديث
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var principal = JwtHelper.GetPrincipalFromExpiredToken(request.Token, _config);
        if (principal == null) return BadRequest(new { Success = false, Message = "التوكن غير صالح" });

        var userIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            return BadRequest(new { Success = false, Message = "التوكن غير صالح" });

        var result = await _authService.RefreshTokenAsync(userId, request.RefreshToken);
        if (!result.Success) return BadRequest(result);

        var newToken = JwtHelper.GenerateToken(result.Data!.UserId, result.Data.FullName, "User", _config);
        var newRefreshToken = JwtHelper.GenerateRefreshToken();

        await SaveRefreshTokenAsync(userId, newRefreshToken);

        return Ok(new { Success = true, Message = "تم تحديث التوكن بنجاح", Data = result.Data with { Token = newToken, RefreshToken = newRefreshToken } });
    }

    // تسجيل الخروج (إبطال رمز التحديث)
    [Authorize]
    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken()
    {
        var result = await _authService.RevokeRefreshTokenAsync(GetUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // حفظ رمز التحديث في قاعدة البيانات
    private async Task SaveRefreshTokenAsync(int userId, string refreshToken)
    {
        var user = await _uow.Repository<AppUser>().Query().FirstOrDefaultAsync(u => u.Id == userId);
        if (user != null)
        {
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(30);
            _uow.Repository<AppUser>().Update(user);
            await _uow.SaveChangesAsync();
        }
    }
}
