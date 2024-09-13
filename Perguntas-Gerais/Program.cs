using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Mvc_MongoDB.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
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

app.MapPost(
    "/resposta",
    async (PerguntaService perguntaService, [FromBody] Perguntas novaPergunta) =>
    {
        var perguntaCriada = await perguntaService.InsertRespostaAsync(novaPergunta);

        return Results.Ok(
            $"Respostas gravadas com sucesso: {perguntaCriada.idpergunta}, {perguntaCriada.resposta}"
        );
    }
);

app.MapGet(
    "/perguntas",
    (NovaPerguntaService novaPerguntaService) =>
    {
        return novaPerguntaService.FindPerguntas();
    }
);

app.MapPost(
    "/pergunta",
    (NovaPerguntaService novaPerguntaService, gravarNovaPergunta novaPerguntanew) =>
    {
        return novaPerguntaService.CreatePerguntaNova(novaPerguntanew);
    }
);

app.MapGet(
    "/respostas",
    (PerguntaService perguntaService) =>
    {
        return perguntaService.GetSum();
    }
);

app.Run();
