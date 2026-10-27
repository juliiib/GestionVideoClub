using System.Text;
using GestionVideoClub;
using GestionVideoClub.Application.Interfaces;
using GestionVideoClub.Application.Services;
using GestionVideoClub.Domain.Entities;
using GestionVideoClub.Domain.Interfaces;
using GestionVideoClub.Infrastructure.ExternalServices;
using GestionVideoClub.Infrastructure.Persistence;
using GestionVideoClub.Infrastructure.Persistence.Seed;
using GestionVideoClub.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pegá acá el token devuelto por POST /api/auth/login (sin la palabra Bearer)."
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, document)] = new List<string>()
    });
});

var connectionString = builder.Configuration.GetConnectionString("GestionVideoClubDb")
    ?? throw new InvalidOperationException("La connection string 'GestionVideoClubDb' no está configurada.");

builder.Services.AddDbContext<VideoClubDbContext>(o => o.UseSqlServer(connectionString));

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>();
if (jwtSettings == null || string.IsNullOrWhiteSpace(jwtSettings.Key))
{
    throw new InvalidOperationException("La sección 'Jwt' (o su clave 'Key') no está configurada.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.EmployeesOnly,
        policy => policy.RequireRole(nameof(Employee)));

    options.AddPolicy(Policies.EmployeesAndClients,
        policy => policy.RequireRole(nameof(Employee), nameof(Client)));
});

builder.Services.Configure<OmdbSettings>(builder.Configuration.GetSection(OmdbSettings.SectionName));

var omdbSettings = builder.Configuration.GetSection(OmdbSettings.SectionName).Get<OmdbSettings>();
if (omdbSettings == null || string.IsNullOrWhiteSpace(omdbSettings.ApiKey))
{
    throw new InvalidOperationException(
        "La sección 'Omdb' (o su clave 'ApiKey') no está configurada. Configurala con: " +
        "dotnet user-secrets set \"Omdb:ApiKey\" \"<tu-key>\" --project GestionVideoClub");
}

builder.Services.AddHttpClient<IMovieInfoService, OmdbMovieInfoService>(client =>
{
    client.BaseAddress = new Uri(omdbSettings.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IClientService, ClientService>();

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IMovieService, MovieService>();

builder.Services.AddScoped<IFormatRepository, FormatRepository>();
builder.Services.AddScoped<IFormatService, FormatService>();

builder.Services.AddScoped<ICopyRepository, CopyRepository>();
builder.Services.AddScoped<ICopyService, CopyService>();

builder.Services.AddScoped<IRentRepository, RentRepository>();
builder.Services.AddScoped<IRentService, RentService>();

builder.Services.AddScoped<IFineRepository, FineRepository>();
builder.Services.AddScoped<IFineService, FineService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IPasswordHashService, BCryptPasswordHashService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // VideoClubDbContext e IPasswordHashService están registrados como Scoped, pero acá, en el
    // arranque, no hay un request HTTP que abra un scope por nosotros. CreateScope() crea uno a
    // mano para poder resolverlos, y se descarta (using) apenas termina de sembrar los datos.
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<VideoClubDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHashService>();
        await context.Database.MigrateAsync();
        await DbSeeder.SeedAsync(context, hasher);
    }
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
