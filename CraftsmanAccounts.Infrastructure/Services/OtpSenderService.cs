// خدمة إرسال رمز التحقق عبر واتساب باستخدام بوابة Standing Tech
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CraftsmanAccounts.Infrastructure.Services;

public class OtpSenderService : IOtpSenderService
{
    private readonly HttpClient _httpClient;
    private readonly string _senderId;
    private readonly ILogger<OtpSenderService> _logger;

    public OtpSenderService(HttpClient httpClient, IConfiguration config, ILogger<OtpSenderService> logger)
    {
        _httpClient = httpClient;
        _senderId = config["SmsGateway:SenderId"] ?? "CraftsmanAccounts";
        _logger = logger;
    }

    public async Task<ServiceResult> SendAsync(string phoneNumber, string otpCode)
    {
        try
        {
            // تحويل رقم الهاتف من الصيغة المحلية إلى الصيغة الدولية
            var recipient = ConvertToInternational(phoneNumber);

            var requestData = new
            {
                recipient,
                sender_id = _senderId,
                type = "whatsapp",
                message = $"رمز التحقق الخاص بك هو: {otpCode}",
                lang = "ar"
            };

            var json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/v4/sms/send", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObj = JsonSerializer.Deserialize<SmsApiResponse>(responseContent);

                if (responseObj?.status == "success")
                    return ServiceResult.Ok("تم إرسال رمز التحقق بنجاح");
            }

            _logger.LogWarning("فشل إرسال OTP للرقم {Phone}، كود الاستجابة: {StatusCode}", phoneNumber, response.StatusCode);
            return ServiceResult.Fail("فشل إرسال رمز التحقق، يرجى المحاولة مرة أخرى");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء إرسال OTP للرقم {Phone}", phoneNumber);
            return ServiceResult.Fail("حدث خطأ أثناء إرسال رمز التحقق");
        }
    }

    // تحويل 07XXXXXXXX إلى 9647XXXXXXXX
    private static string ConvertToInternational(string phoneNumber)
    {
        if (phoneNumber.StartsWith('0'))
            return $"964{phoneNumber[1..]}";
        if (phoneNumber.StartsWith("+964"))
            return phoneNumber[1..];
        if (phoneNumber.StartsWith("964"))
            return phoneNumber;
        return phoneNumber;
    }

    // كائنات استجابة بوابة الرسائل
    private sealed class SmsApiResponse
    {
        public string status { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
    }
}
