using GestionVideoClub.Application.Interfaces;
using GestionVideoClub.Application.Services;
using GestionVideoClub.Domain.Interfaces;
using GestionVideoClub.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IClientService, ClientService>();

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
