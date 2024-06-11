using sistema_barbearia.Models;
using Microsoft.EntityFrameworkCore;
using sistema_barbearia.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class AgendamentoService : IAgendamentoService
{
    private readonly AppDbContext _context;

    public AgendamentoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Agendamento>> GetAllAsync()
    {
        return await _context.Agendamentos.Include(a => a.User).Include(a => a.Barbeiro).ToListAsync();
    }

    public async Task<Agendamento> GetByIdAsync(int id)
    {
        return await _context.Agendamentos.Include(a => a.User).Include(a => a.Barbeiro).FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Agendamento> CreateAsync(Agendamento agendamento)
    {
        // Verifica se o usuário cliente já possui um agendamento para o mesmo dia
        var existingAgendamentoForUser = await _context.Agendamentos
            .Where(a => a.UserId == agendamento.UserId && a.Horario.Date == agendamento.Horario.Date)
            .FirstOrDefaultAsync();

        if (existingAgendamentoForUser != null)
        {
            // Se o usuário cliente já tiver um agendamento para o mesmo dia, retorne null ou lance uma exceção
            throw new InvalidOperationException("O usuário já possui um agendamento para este dia.");   
        }
        
        agendamento.Status = StatusAgendamento.Disponivel;
        _context.Agendamentos.Add(agendamento);
        await _context.SaveChangesAsync();
        return agendamento;
    }

    public async Task<Agendamento> UpdateAsync(int id, Agendamento agendamento)
    {
        var existingAgendamento = await _context.Agendamentos.FindAsync(id);
        if (existingAgendamento == null) return null;

        existingAgendamento.Nome = agendamento.Nome;
        existingAgendamento.TipoCorte = agendamento.TipoCorte;
        existingAgendamento.Preco = agendamento.Preco;
        existingAgendamento.Horario = agendamento.Horario;
        existingAgendamento.Status = agendamento.Status;
        existingAgendamento.UserId = agendamento.UserId;
        existingAgendamento.BarbeiroId = agendamento.BarbeiroId;
        existingAgendamento.UpdatedAt = DateTime.Now;

        _context.Agendamentos.Update(existingAgendamento);
        await _context.SaveChangesAsync();
        return existingAgendamento;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var agendamento = await _context.Agendamentos.FindAsync(id);
        if (agendamento == null) return false;

        _context.Agendamentos.Remove(agendamento);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Agendamento> ReserveAsync(int id, int userId)
    {
        var agendamento = await _context.Agendamentos.FindAsync(id);
        if (agendamento == null || agendamento.Status != StatusAgendamento.Disponivel) return null;

        agendamento.Status = StatusAgendamento.Reservado;
        agendamento.UserId = userId;
        agendamento.UpdatedAt = DateTime.Now;

        _context.Agendamentos.Update(agendamento);
        await _context.SaveChangesAsync();
        return agendamento;
    }

    public async Task<Agendamento> CancelAsync(int id)
    {
        var agendamento = await _context.Agendamentos.FindAsync(id);
        if (agendamento == null) return null;

        agendamento.Status = DateTime.Now > agendamento.Horario ? StatusAgendamento.Cancelado : StatusAgendamento.Disponivel;
        agendamento.UserId = null;
        agendamento.UpdatedAt = DateTime.Now;

        _context.Agendamentos.Update(agendamento);
        await _context.SaveChangesAsync();
        return agendamento;
    }

    public void UpdateStatus(Agendamento agendamento)
    {
        var now = DateTime.Now;
        if (agendamento.Status == StatusAgendamento.Reservado && now >= agendamento.Horario && now <= agendamento.Horario.AddMinutes(30))
        {
            agendamento.Status = StatusAgendamento.Andamento;
        }
        else if (agendamento.Status == StatusAgendamento.Andamento && now > agendamento.Horario.AddMinutes(30))
        {
            agendamento.Status = StatusAgendamento.Cancelado;
        }

        _context.Agendamentos.Update(agendamento);
        _context.SaveChanges();
    }
}
