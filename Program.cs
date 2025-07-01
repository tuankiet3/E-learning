using E_learning.DAL.Course;
using E_learning.DAL.Auth;
using E_learning.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using E_learning.Repositories.Course;
using E_learning.Services.VNPay;
using E_learning.Repositories.Auth;

var builder = WebApplication.CreateBuilder(args);

// Lấy chuỗi kết nối
var connectionString = builder.Configuration.GetConnectionString("SqlServerConnection");
// Đăng kí VNPayService
builder.Services.AddScoped<VnPayService>();
// Đăng ký các DAL
builder.Services.AddSingleton(provider => new CoursesDAL(connectionString, provider.GetRequiredService<ILogger<CoursesDAL>>()));
builder.Services.AddSingleton(provider => new LessonDAL(connectionString, provider.GetRequiredService<ILogger<LessonDAL>>()));
builder.Services.AddSingleton(provider => new QuizDAL(connectionString, provider.GetRequiredService<ILogger<QuizDAL>>()));
builder.Services.AddSingleton(provider => new ChoiceDAL(connectionString, provider.GetRequiredService<ILogger<ChoiceDAL>>()));
builder.Services.AddSingleton(provider => new AuthDAL(connectionString, provider.GetRequiredService<ILogger<AuthDAL>>()));


// Đăng ký Repository và các service khác
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<GenerateID>();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IZoomService, ZoomService>();


// === CẤU HÌNH JWT AUTHENTICATION ===
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// === THAY THẾ CẤU HÌNH SWAGGERGEN TẠI ĐÂY ===
builder.Services.AddSwaggerGen(options =>
{
    // 1. Định nghĩa Security Scheme (cách xác thực)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\n\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\""
    });

    // 2. Thêm Security Requirement (yêu cầu xác thực)
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
// ============================================

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
