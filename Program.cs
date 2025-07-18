﻿using E_learning.DAL.Auth;
using E_learning.DAL.Course;
using E_learning.Repositories.Auth;
using E_learning.Repositories.Course;
using E_learning.Repositories.Enrollment;
using E_learning.DAL.Enrollment;
using E_learning.Services;
using E_learning.Services.VNPay;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using E_learning.Repositories.Payment;
using E_learning.DAL.Payment;
using E_learning.Services.Zoom;
using E_learning.Services.Lesson;
using E_learning.Services.Lesson;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<CoursesDAL>();
builder.Services.AddSingleton<LessonDAL>();
builder.Services.AddSingleton<QuizDAL>();
builder.Services.AddSingleton<ChoiceDAL>();
builder.Services.AddSingleton<AuthDAL>();
builder.Services.AddSingleton<PaymentDAL>();
builder.Services.AddSingleton<EnrollmentDAL>();
builder.Services.AddSingleton<QuestionDAL>();

// Đăng ký Repository và các service khác
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<GenerateID>();
builder.Services.AddScoped<CheckExsistingID>();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IZoomService, ZoomService>();
builder.Services.AddScoped<VnPayService>();

// Cấu hình HttpClientFactory cho Zoom
builder.Services.AddHttpClient("Zoom", client =>
{
    client.BaseAddress = new Uri("https://api.zoom.us/v2/");
});

// Cấu hình JWT Authentication
builder.Services.AddSingleton<CoursesDAL>();
builder.Services.AddSingleton<LessonDAL>();
builder.Services.AddSingleton<QuizDAL>();
builder.Services.AddSingleton<ChoiceDAL>();
builder.Services.AddSingleton<AuthDAL>();
builder.Services.AddSingleton<PaymentDAL>();
builder.Services.AddSingleton<EnrollmentDAL>();
builder.Services.AddSingleton<QuestionDAL>();
// Đăng ký Repository và các service khác
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
//builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<GenerateID>();
builder.Services.AddScoped<CheckExsistingID>();
builder.Services.AddScoped<Normalize>();
builder.Services.AddScoped<VnPayService>();
builder.Services.AddScoped<VnPayLibrary>();
builder.Services.AddScoped<ConvertURL>();
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

// Cấu hình Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\n\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\""
    });
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