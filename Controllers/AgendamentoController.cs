using sistema_barbearia.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class AgendamentoController : Controller
{
    private readonly IAgendamentoService _agendamentoService;

    public AgendamentoController(IAgendamentoService agendamentoService)
    {
        _agendamentoService = agendamentoService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Agendamento>>> GetAll()
    {
        return await _agendamentoService.GetAllAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Agendamento>> GetById(int id)
    {
        var agendamento = await _agendamentoService.GetByIdAsync(id);
        if (agendamento == null) return NotFound();
        return agendamento;
    }

    [HttpPost]
    public async Task<ActionResult<Agendamento>> Create(Agendamento agendamento)
    {
        var createdAgendamento = await _agendamentoService.CreateAsync(agendamento);
        return CreatedAtAction(nameof(GetById), new { id = createdAgendamento.Id }, createdAgendamento);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Agendamento>> Update(int id, Agendamento agendamento)
    {
        var updatedAgendamento = await _agendamentoService.UpdateAsync(id, agendamento);
        if (updatedAgendamento == null) return NotFound();
        return updatedAgendamento;
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var success = await _agendamentoService.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpPost("{id}/reserve")]
    public async Task<ActionResult<Agendamento>> Reserve(int id, [FromQuery] int userId)
    {
        var agendamento = await _agendamentoService.ReserveAsync(id, userId);
        if (agendamento == null) return BadRequest("Agendamento não disponível ou já reservado.");
        return agendamento;
    }

    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<Agendamento>> Cancel(int id)
    {
        var agendamento = await _agendamentoService.CancelAsync(id);
        if (agendamento == null) return NotFound();
        return agendamento;
    }
}
