using E_learning.Repositories;
using E_learning.DAL.Course;
using E_learning.DAL.Auth;
using E_learning.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using E_learning.Repositories.Course;
using E_learning.Services.VNPay;
using E_learning.Repositories.Payment;
using E_learning.Repositories.Enrollment;
using E_learning.DAL.Payment;
using E_learning.DAL.Enrollment;
using E_learning.Services.Lesson;


var builder = WebApplication.CreateBuilder(args);

// Lấy chuỗi kết nối
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
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthentication(); 
app.UseAuthorization(); 

app.MapControllers();

app.Run();