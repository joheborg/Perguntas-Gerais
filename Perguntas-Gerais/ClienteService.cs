using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mvc_MongoDB.Models;

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

    public async Task<Perguntas> CreateClienteAsync(Perguntas perguntas)
    {
        await _perguntas.InsertOneAsync(perguntas);
        return perguntas;
    }
    public List<Resultado> GetSum()
    {

        var resultado = _perguntas.Find(g => g.idpergunta == g.idpergunta).ToList();
        var agrupado = resultado.GroupBy(p => p.idpergunta) // agrupa por idpergunta
                                                            //.Select(g => new Resultado { idpergunta = g.Key, correcao = g.First().correcao }) // seleciona os campos idpergunta e resposta e cria um objeto Resultado
                                .Select(g => new Resultado { idpergunta = g.Key, correcao = g.Sum(p => p.correcao), quantRespostas = g.Count() }) // seleciona os campos idpergunta e correcao e soma os valores de correcao de cada grupo
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

    public async Task<gravarNovaPergunta?> CreatePerguntaNovaAsync(gravarNovaPergunta pergunta)
    {
        var filter = Builders<gravarNovaPergunta>.Filter.Eq(p => p.pergunta, pergunta.pergunta);

        // Define uma atualização para substituir o documento inteiro
        var update = Builders<gravarNovaPergunta>.Update.Set(p => p, pergunta);

        // Define uma opção para retornar o documento atualizado
        var options = new FindOneAndUpdateOptions<gravarNovaPergunta>
        {
            ReturnDocument = ReturnDocument.After,
            IsUpsert = true
        };

        // Executa o método FindOneAndUpdateAsync com a opção upsert: true
        var result = await _pergunta.FindOneAndUpdateAsync(filter, update, options);

        // Verifica se o resultado é nulo, o que significa que a pergunta não existe
        if (result == null)
        {
            // Busca o documento com o maior valor de idpergunta
            var max = await _pergunta.Aggregate().SortByDescending((a) => a.idpergunta).FirstAsync();
            Console.WriteLine(max);
            if (max == null)
            {
                // Define o valor de idpergunta como 1
                pergunta.idpergunta = 1;
            }
            else
            {
                // Define o valor de idpergunta como o máximo mais 1
                pergunta.idpergunta = max.idpergunta + 1;
            }

            // Insere o novo documento na coleção
            await _pergunta.InsertOneAsync(pergunta);

            // Retorna a pergunta inserida
            return pergunta;
        }
        else
        {
            // Retorna a pergunta atualizada
            return result;
        }
    }



}


public class Resultado
{
    public int? idpergunta { get; set; }
    public string? resposta { get; set; }
    public int? correcao { get; set; }
    public int? quantRespostas { get; set; }
}
public class Perguntas
{
    public ObjectId Id { get; set; }
    public int? idpergunta { get; set; }
    public string? resposta { get; set; }
    public int? correcao { get; set; }
}
