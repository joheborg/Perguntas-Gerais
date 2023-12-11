
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
namespace Mvc_MongoDB.Models
{
    public class Resposta
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public int Id { get; set; }

        [Required]
        [Display(Name="idpergunta")]
        public int idpergunta { get; set; }

        [Required]
        [Display(Name="resposta")]
        public String? resposta { get; set; }
        
        [Required]
        [Display(Name="correcao")]
        public int? correcao { get; set; }
        
        [Required]
        [Display(Name="quantRespostas")]
        public int? quantRespostas { get; set; }



    }
}