using System;
using Api.Data.Collections;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

/*
    Objeto do Mongo, com todas as suas configurações...

*/
namespace Api.Data
{
    public class MongoDB
    {
        public IMongoDatabase DB { get; }

        /*
            Tudo que for de configurações de ambiente, incluindo a string de conexão, o nosso 
            Mongo vai receber dessa interface, que é um objeto que o o Program.cs, com suas configurações
            padrão no build do host, vão preenche-lo.
        */
        public MongoDB(IConfiguration configuration)
        {
            try
            {
                // Vamos pegar a connection string.
                var settings = MongoClientSettings.FromUrl(new MongoUrl(Environment.GetEnvironmentVariable("conMongoDB")));
                // Instanciamos um mongo client
                var client = new MongoClient(settings);
                // Aqui também vamos pegar o valor da chave "NomeBanco"
                DB = client.GetDatabase(configuration["NomeBanco"]);
                // Chamamos esse método
                MapClasses();
            }
            catch (Exception ex)
            {
                throw new MongoException("It was not possible to connect to MongoDB", ex);
            }
        }

        private void MapClasses()
        {
            // Vamos criar uma convenção para que o mongo trabalhe com CamelCase
            var conventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("camelCase", conventionPack, t => true);

            // E caso não tenhamos nada mapeado,
            if (!BsonClassMap.IsClassMapRegistered(typeof(Produtos)))
            {

                BsonClassMap.RegisterClassMap<Produtos>(i =>
                {
                /*
                    Aqui, vamos mapear no banco, com o mesmo nome e tipo da classe que fizemos.
                */
                    i.AutoMap();
                    // Para caso você apartir de hoje tenha uma nova coluna no documento, e o banco não estranhe...
                    i.SetIgnoreExtraElements(true);
                });
            }
        }
    }
}