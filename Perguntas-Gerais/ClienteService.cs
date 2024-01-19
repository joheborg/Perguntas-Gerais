using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mvc_MongoDB.Models;
using static NovaPerguntaService;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

public class PerguntaService
{
    private readonly IMongoCollection<Perguntas> _perguntas;

    public PerguntaService(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("main");
        _perguntas = database.GetCollection<Perguntas>("respostas");
    }

    public async Task<IEnumerable<Perguntas>> GetClientesAsync()
    {
        return await _perguntas.Find(perguntas => true).ToListAsync();
    }

    public async Task<Perguntas> InsertRespostaAsync(Perguntas perguntas)
    {
        await _perguntas.InsertOneAsync(perguntas);
        return perguntas;
    }
    public List<Resultado> GetSum()
    {
        var resultado = _perguntas
            .Find(g => g.idpergunta == g.idpergunta && g.resposta == g.correcao.ToString()).ToList();
        var agrupado = resultado
            .GroupBy(p => p.idpergunta) // agrupa por idpergunta
            .Select(g => new Resultado { idpergunta = g.Key, quantRespostas = g.Count() }) // seleciona os campos idpergunta e correcao e soma os valores de correcao de cada grupo
            .ToList(); // converte para uma lista
        return agrupado;
    }

}
public class NovaPerguntaService
{
    private readonly IMongoCollection<gravarNovaPergunta> _pergunta;

    public NovaPerguntaService(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("main");
        _pergunta = database.GetCollection<gravarNovaPergunta>("pergunta");
    }

    public List<gravarNovaPergunta> CreatePerguntaNova(gravarNovaPergunta novaPergunta)
    {
        var resultado = _pergunta.Find(g => g.pergunta == novaPergunta.pergunta).ToList();

        if (resultado[0].Id!.Length > 0)
        {
            var filter = Builders<gravarNovaPergunta>.Filter
                .Eq("pergunta", resultado[0].pergunta);
            var update = Builders<gravarNovaPergunta>.Update
                .Set("resposta1", novaPergunta.resposta1)
                .Set("resposta2", novaPergunta.resposta2)
                .Set("resposta3", novaPergunta.resposta3)
                .Set("resposta4", novaPergunta.resposta4)
                .Set("correta", novaPergunta.correta);
            _pergunta.UpdateOne(filter, update);
            var resultadoupdate = _pergunta.Find(g => g.pergunta == novaPergunta.pergunta).ToList();
            return resultadoupdate;
        }
        _pergunta.InsertOneAsync(novaPergunta);
        var resultadoinsert = _pergunta.Find(g => g.pergunta == novaPergunta.pergunta).ToList();
        return resultadoinsert;
    }
    public List<gravarNovaPergunta> FindPerguntas()
    {
        return _pergunta.Find(perguntas => true).ToList();
    }

}


public class Resultado
{
    public string? idpergunta { get; set; }
    public string? resposta { get; set; }
    public int? correcao { get; set; }
    public int? quantRespostas { get; set; }
}

public class Perguntas
{
    public ObjectId Id { get; set; }
    public string? idpergunta { get; set; }
    public string? resposta { get; set; }
    public int? correcao { get; set; }

}
