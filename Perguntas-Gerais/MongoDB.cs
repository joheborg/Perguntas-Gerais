using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;

namespace Mvc_MongoDB.Models
{
    public class RespostasDB
    {
        public string DataBaseName { get; } = "main";
        public IMongoDatabase Database { get; }
        public IMongoCollection<Resposta> Respostas => Database.GetCollection<Resposta>("respostas");
        public IMongoCollection<gravarNovaPergunta> Pergunta => Database.GetCollection<gravarNovaPergunta>("pergunta");
        public MongoClient Cliente { get; }

        public RespostasDB(IConfiguration configuration)
        {
            // usar IConfiguration para obter a string de conexão
            string? conexaoMongoDB = configuration.GetConnectionString("myConnectionString");
            Cliente = new MongoClient(conexaoMongoDB);
            Database = Cliente.GetDatabase(DataBaseName);
        }
    }

    [BsonIgnoreExtraElements]
    public class gravarNovaPergunta
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? pergunta { get; set; }
        public string? resposta1 { get; set; }
        public string? resposta2 { get; set; }
        public string? resposta3 { get; set; }
        public string? resposta4 { get; set; }
        public string? correta { get; set; }
        public string? obs { get; set; }

    }

}
