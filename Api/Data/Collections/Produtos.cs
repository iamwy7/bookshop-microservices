using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

/* 
    Aqui, temos o que será mapeado para ser uma Collection ( o que seria uma tabela
nos bancos relacionais ) no mongo. Mas, Porque ?:

    Porque precisamos gravar no mongo, num formato especifico de geolocalização, para 
tratamentos de dados futuros.

    Em suma, essa classe, é a representação da collection, no banco.

    Uma doc que pode ajudar no entendimento: https://docs.mongodb.com/drivers/csharp
*/
namespace Api.Data.Collections
{
    public class Produtos
    {
        public Produtos(string nomeLivro, DateTime dataPub, int isbn, string nomeAutor, double precoLivro)
        {
            NomeLivro = nomeLivro;
            DataPub = dataPub;
            ISBN = isbn;
            NomeAutor = nomeAutor;
            PrecoLivro = precoLivro;
        }
        
        #region Livros
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }        
        public string NomeLivro { get; set; }
        public DateTime DataPub { get; set; }
        public int ISBN { get; set; }
        public string NomeAutor { get; set; }
        public double PrecoLivro { get; set; }
        #endregion
    }
}
