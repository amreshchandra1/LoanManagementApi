using JWTAuthentication;
using LoanManagementApi;
using LoanManagementApi.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>
option.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidIssuer = "mytokengenerationapp",
    ValidAudience = "myclientapp",
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("AmreshChadraSecretKeyJWTAmreshChadraSecretKeyJWT")),
    ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 }
}

);
builder.Services.AddControllers().AddJsonOptions(options =>
{
    // This converts enums to strings globally across the API and Swagger UI
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IGenerateToken, GenerateToken>();
builder.Services.AddScoped<ILoan, Loan>();
builder.Services.AddScoped<ILogin, Login>();
builder.Services.AddScoped<IHelper, Helper>();
builder.Services.AddScoped<IAuditLog, AuditLogRepository>();
builder.Services.AddScoped<IRoleManagement, RoleManagement>();
var connectionString = builder.Configuration.GetConnectionString("SQLConnection");

builder.Services.AddDbContext<EFContext>(
    options => options.UseSqlServer(connectionString)
);
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Host.UseSerilog((hostingContext, loggerConfig) =>
{
    if (hostingContext.HostingEnvironment.IsDevelopment())//Added only for dev environment.By using this line serilog will only work for dev environment
    {
        loggerConfig.ReadFrom.Configuration(hostingContext.Configuration);
    }
    else
    {
        loggerConfig
                    .MinimumLevel.Debug()
                    .WriteTo.Console();
    }
}
);
var app = builder.Build();

app.UseDirectoryBrowser();
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseSwagger();
app.UseSwaggerUI();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    // TEMPORARY: Un-comment this line to force Azure to show the real error details
    app.UseDeveloperExceptionPage();

  //  app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
