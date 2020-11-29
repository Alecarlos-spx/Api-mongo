using System;
using Api.Data.Collections;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InfectadoController : ControllerBase
    {
        Data.MongoDB _mongoDB;
        IMongoCollection<Infectado> _infectadosCollection;

        public InfectadoController(Data.MongoDB mongoDB)
        {
            _mongoDB = mongoDB;
            _infectadosCollection = _mongoDB.DB.GetCollection<Infectado>(typeof(Infectado).Name.ToLower());
        }

        [HttpPost]
        public ActionResult SalvarInfectado([FromBody] InfectadoDto dto)
        {
            var infectado = new Infectado(dto.Id, dto.Nome, dto.DataNascimento, dto.Sexo, dto.Latitude, dto.Longitude);

            _infectadosCollection.InsertOne(infectado);
            
            return StatusCode(201, "Infectado adicionado com sucesso");
        }

        [HttpGet]
        public ActionResult ObterInfectados()
        {
            var infectados = _infectadosCollection.Find(Builders<Infectado>.Filter.Empty).ToList();
            
            return Ok(infectados);
        }

        [HttpPut("{id}")]
        public ActionResult AtualizarInfectado(string id, [FromBody] InfectadoDto dto)
        {

            Infectado infectadoCollection = ConvertDtoEmInfectado(dto,id);

           var resultado =  _infectadosCollection.Find<Infectado>(infectado => infectado.Id == id).FirstOrDefault();

            if (resultado == null)
                return NotFound();
            

            _infectadosCollection.ReplaceOne(infectado => infectado.Id == id, infectadoCollection);

            return Ok("Atualizado com sucesso");
        }

        [HttpDelete("{id}")]
        public ActionResult ApagarInfectado(string id)
        {
             _infectadosCollection.DeleteOne(Builders<Infectado>.Filter.Where(_ => 
                _.Id == id));

            return Ok("Apagado com sucesso");
        }

           private static Infectado ConvertDtoEmInfectado(InfectadoDto infectado, string id)
        {
            return new Infectado(id, infectado.Nome, infectado.DataNascimento, infectado.Sexo, infectado.Latitude, infectado.Longitude);
        }



    }
}
