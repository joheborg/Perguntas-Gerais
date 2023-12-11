using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Mvc_MongoDB.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

// Registre IMongoClient no contêiner de injeção de dependência
string? conexaoMongoDB = builder.Configuration.GetConnectionString("myConnectionString");
builder.Services.AddSingleton<IMongoClient>(new MongoClient(conexaoMongoDB));
builder.Services.AddScoped<PerguntaService>();
builder.Services.AddScoped<NovaPerguntaService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors();

app.UseHttpsRedirection();

app.MapGet("/gravarresposta/{idpergunta}/{resposta}/{correcao}", async (PerguntaService perguntaService, int idpergunta, string resposta, int correcao) =>
{
    var novaPergunta = new Perguntas { idpergunta = idpergunta, resposta = resposta, correcao = correcao };
    var perguntaCriada = await perguntaService.CreateClienteAsync(novaPergunta);

    return $"Respostas gravadas com sucesso: {perguntaCriada.idpergunta}, {perguntaCriada.resposta}";
});
app.MapPost("/gravarnovapergunta", async (NovaPerguntaService novaPerguntaService, gravarNovaPergunta novaPergunta) =>
{
    var perguntaCriada = await novaPerguntaService.CreatePerguntaNovaAsync(novaPergunta);
    return perguntaCriada;
});

app.MapGet("/verificarrespostas", (PerguntaService perguntaService) =>
{
    var perguntaCriada = perguntaService.GetSum();

    return perguntaCriada;
});

app.Run();



