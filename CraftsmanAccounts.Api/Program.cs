// نقطة بداية واجهة البرمجة - إعداد المصادقة JWT والخدمات وقاعدة البيانات وإشعارات OneSignal وتوثيق Swagger
using System.Text;
using CraftsmanAccounts.Application;
using CraftsmanAccounts.Application.Interfaces;
using CraftsmanAccounts.Infrastructure;
using CraftsmanAccounts.Infrastructure.Seeding;
using CraftsmanAccounts.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// إضافة طبقات التطبيق والبنية التحتية
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// إعداد خدمة إشعارات OneSignal عبر HttpClient
builder.Services.AddHttpClient<IUserNotificationService, OneSignalService>(client =>
{
    client.BaseAddress = new Uri("https://api.onesignal.com/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    var apiKey = builder.Configuration["OneSignal:RestApiKey"] ?? "";
    client.DefaultRequestHeaders.Add("Authorization", $"Key {apiKey}");
});

// إعداد مصادقة JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? "CraftsmanAccountsSuperSecretKeyThatIs256BitsLong!!";
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "CraftsmanAccounts",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "CraftsmanAccountsUsers",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers(opt =>
{
    opt.Filters.Add<CraftsmanAccounts.Api.Filters.ActivityLogFilter>();
});
builder.Services.AddEndpointsApiExplorer();

// إعداد Swagger مع دعم مصادقة JWT
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CraftsmanAccounts API",
        Version = "v1",
        Description = "واجهة برمجة تطبيقات نظام حسابات الحرفي - إدارة العمال والمشاريع والمالية"
    });

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "أدخل التوكن JWT فقط بدون كلمة Bearer"
    });

    opt.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", doc),
            new List<string>()
        }
    });
});

// إعداد CORS
builder.Services.AddCors(opt => opt.AddDefaultPolicy(p => p
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()));

var app = builder.Build();

// تهيئة وبذر قاعدة البيانات
await DbSeeder.SeedAsync(app.Services);

// تفعيل Swagger في جميع البيئات
app.UseSwagger();
app.UseSwaggerUI(opt =>
{
    opt.SwaggerEndpoint("/swagger/v1/swagger.json", "CraftsmanAccounts API v1");
    opt.RoutePrefix = "swagger";
});

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// ميدل وير التحقق من اشتراك المستخدم
app.UseMiddleware<CraftsmanAccounts.Api.Middleware.SubscriptionExpiryMiddleware>();

app.MapControllers();

app.Run();
