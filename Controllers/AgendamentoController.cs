using sistema_barbearia.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using sistema_barbearia.DTOs;



namespace sistema_barberia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AgendamentoController : Controller
    {
        private readonly IAgendamentoService _agendamentoService;

        public AgendamentoController(IAgendamentoService agendamentoService)
        {
            _agendamentoService = agendamentoService;
        }

        [HttpGet("/Listagem")]
        public async Task<IActionResult> Listagem()
        {

            var agendamentos = await _agendamentoService.GetAllAsync();

            return View("Listagem", agendamentos);
        }

        [HttpGet("/ListagemClientes")]
        public async Task<IActionResult> ListagemClientes()
        {
            var agendamentos = await _agendamentoService.GetAllAsync();
            return View("ListagemClientes", agendamentos);
        }

        [HttpGet]
        public async Task<ActionResult<List<Agendamento>>> GetAll()
        {
            var agendamentosDto = await _agendamentoService.GetAllAsync2();

            // Convertendo AgendamentoDto para Agendamento
            var agendamentos = agendamentosDto.Select(dto => new Agendamento
            {
                Id = dto.Id,
                Nome = dto.Nome,
                TipoCorte = dto.TipoCorte,
                Preco = dto.Preco,
                Horario = dto.Horario,
                UserId = dto.Usuario.Id 
                
            }).ToList();

            return agendamentos;
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Agendamento>> GetById(int id)
        {
            var agendamento = await _agendamentoService.GetByIdAsync(id);
            if (agendamento == null) return NotFound();
            return agendamento;
        }

        [HttpPost]
        public async Task<ActionResult<Agendamento>> Create([FromBody] CreateAgendamentoDto agendamentoDto)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");

                if (!userId.HasValue)
                {
                    return Unauthorized("Usuário não autenticado");
                }

                var createdAgendamento = await _agendamentoService.CreateAsync(agendamentoDto, userId.Value);
                return CreatedAtAction(nameof(GetById), new { id = createdAgendamento.Id }, new
                {
                    createdAgendamento.Id,
                    UserId = createdAgendamento.UserId,
                    UserName = createdAgendamento.User.UserName,
                    Preco = createdAgendamento.Preco,
                    Status = createdAgendamento.Status

                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // [HttpPut("{id}")]
        // public async Task<ActionResult<Agendamento>> Update(int id, Agendamento agendamento)
        // {
        //     var updatedAgendamento = await _agendamentoService.UpdateAsync(id, agendamento);
        //     if (updatedAgendamento == null) return NotFound();
        //     return updatedAgendamento;
        // }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var success = await _agendamentoService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        // [HttpPost("{id}/reserve")]
        // public async Task<ActionResult<Agendamento>> Reserve(int id, [FromQuery] int userId)
        // {
        //     var agendamento = await _agendamentoService.ReserveAsync(id, userId);
        //     if (agendamento == null) return BadRequest("Agendamento não disponível ou já reservado.");
        //     return agendamento;
        // }

        // [HttpPost("{id}/cancel")]
        // public async Task<ActionResult<Agendamento>> Cancel(int id)
        // {
        //     var agendamento = await _agendamentoService.CancelAsync(id);
        //     if (agendamento == null) return NotFound();
        //     return agendamento;
        // }

        // AgendamentoController

        // [HttpGet("/UpdateStatus")]
        // public async Task<IActionResult> UpdateStatus()
        // {
        //     var agendamentos = await _agendamentoService.GetAllAsync();
        //     foreach (var agendamento in agendamentos)
        //     {
        //         _agendamentoService.UpdateStatus(agendamento);
        //     }
        //     return Ok("Status updated successfully.");
        // }

        [HttpGet("GetPrecoByTipoCorte")]
        public ActionResult<decimal> GetPrecoByTipoCorte([FromQuery] string tipoCorte)
        {
            try
            {
                var preco = _agendamentoService.GetPrecoByTipoCorte(tipoCorte);
                return Ok(preco);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
