using Api.Data.Collections;
using Api.Controllers;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;

namespace Api.Controllers
{
    // Informamos que é uma API Controller
    [ApiController]
    // Desse jeito, ele desconsidera o "controller" do nome da rota, fica "Produtos" mesmo.
    [Route("[controller]")]
    public class ProdutosController : ControllerBase
    {
        // Declaramos um mongo pra gente.
        private readonly Data.MongoDB _mongoDB;
        // Declaramos uma coleção do mongo pra gente também.
        private readonly IMongoCollection<Produtos> _produtosCollection;

        // E na instancia desse Controller, recebemos por injeção de dependencia, uma instancia do mongo ( no Startup.cs ).
        public ProdutosController(Data.MongoDB mongoDB)
        {
            // Iniciamos o Mongo
            _mongoDB = mongoDB;
            // E dizemos com qual coleção ele vai trabalhar, a Collection.Produtos ...
            _produtosCollection = _mongoDB.DB.GetCollection<Produtos>(typeof(Produtos).Name.ToLower());
        }

        [HttpGet]
        public ActionResult GetAll()
        {
            var Produtos = _produtosCollection.Find(Builders<Produtos>.Filter.Empty).ToList();
            
            return Ok(Produtos);
        }
        
        [HttpGet("{id:length(24)}")]
        public ActionResult GetById(string id)
        {
            var Produtos = _produtosCollection.Find(Builders<Produtos>.Filter.Empty).FirstOrDefault();
            
            return Ok(Produtos);
        }


        // Daqui pra baixo é REST
        [HttpPost]
        // Vai receber um objeto com a estrutura daquela DTO
        public ActionResult Post([FromBody] ProdutosDto dto)
        {
            // Instanciamos o que será o documento.
            var Produtos = new Produtos(dto.NomeLivro, dto.DataPub, dto.ISBN, dto.NomeAutor, dto.PrecoLivro);
            // E adicionamos na nossa coleção ali de cima.
            _produtosCollection.InsertOne(Produtos);
            // Retornamos essa ação
            return StatusCode(201, "Adicionado com sucesso");

            // Feito muleque. 
            // Se você gosta de gatos, e quiser saber mais sobre esse tipo de retorno: https://http.cat/201
        }
        
        [HttpPut("{id:length(24)}")]
        // Vai receber um objeto com a estrutura daquela DTO
        public ActionResult Update(string id, [FromBody] ProdutosDto dto)
        {        
            // Editamos, filtrando por Data.
            _produtosCollection.UpdateOne(Builders<Produtos>.Filter.Where(_ => _.Id == id), Builders<Produtos>.Update.Set("precoLivro",dto.PrecoLivro));
            // Retornamos essa ação
            return Ok("Atualizado com Sucesso");
            // Feito também
        }

        [HttpDelete("{id:length(24)}")]
        // Vai receber um objeto com a estrutura daquela DTO
        public ActionResult Delete(string id)
        {        
            // Vamos procurar por data por enquanto.
            _produtosCollection.DeleteOne(Builders<Produtos>.Filter.Where(_ => _.Id == id));
            return Ok("Deletado com Sucesso");
            // Feito também
        }
    }
}
