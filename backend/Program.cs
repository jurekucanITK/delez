using System.Text;
using Delez.Api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DelezDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Privzeta")));

var jwt = builder.Configuration.GetSection("Jwt");
var kljuc = jwt["Kljuc"];
if (string.IsNullOrWhiteSpace(kljuc))
{
    throw new InvalidOperationException("Nastavitev Jwt:Kljuc manjka.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt["Izdajatelj"],
            ValidateAudience = true,
            ValidAudience = jwt["Obcinstvo"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(kljuc)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });
builder.Services.AddAuthorization();

const string CorsPolitika = "Frontend";
var dovoljeniIzvori = builder.Configuration.GetSection("Cors:DovoljeniIzvori").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
    options.AddPolicy(CorsPolitika, policy =>
        policy.WithOrigins(dovoljeniIzvori)
            .AllowAnyHeader()
            .AllowAnyMethod()));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Delež API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Vnesi JWT žeton brez predpone Bearer."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(CorsPolitika);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
