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

    public class gravarNovaPergunta
    {  
        // [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public int? idpergunta { get; set; }
        public string? pergunta { get; set; }
        public string? resposta1 { get; set; }
        public string? resposta2 { get; set; }
        public string? resposta3 { get; set; }
        public string? resposta4 { get; set; }
        // usar o atributo BsonElement para especificar o nome do campo no documento
        [BsonElement("correta")]
        public int correta { get; set; }
        
    }
}
