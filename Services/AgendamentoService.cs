using sistema_barbearia.Models;
using Microsoft.EntityFrameworkCore;
using sistema_barbearia.Data;
using sistema_barbearia.DTOs;
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
        return await _context.Agendamentos.Include(a => a.User).ToListAsync();
    }

    public async Task<List<AgendamentoDto>> GetAllAsync2()
    {
        var agendamentos = await _context.Agendamentos
            .Include(a => a.User)
            .Select(a => new AgendamentoDto
            {
                Id = a.Id,
                Nome = a.Nome,
                TipoCorte = a.TipoCorte,
                Preco = a.Preco,
                Horario = a.Horario,
                Usuario = new UserDto
                {
                    Id = a.User.Id,
                    Username = a.User.UserName
                },
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();

        return agendamentos;
    }


    public async Task<Agendamento> GetByIdAsync(int id)
    {
        var agendamento = await _context.Agendamentos.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == id);
        return agendamento;
    }

    public async Task<Agendamento> CreateAsync(CreateAgendamentoDto agendamentoDto, int userId)
    {
        if (agendamentoDto.Horario < DateTime.Now)
        {
            throw new InvalidOperationException("Não é possível agendar para um horário que já passou.");
        }

        var existingAgendamentoForTime = await _context.Agendamentos
        .Where(a => a.Horario == agendamentoDto.Horario)
        .FirstOrDefaultAsync();

        if (existingAgendamentoForTime != null)
        {
            throw new InvalidOperationException("Já existe um agendamento para este horário.");
        }

        var existingAgendamentoForUser = await _context.Agendamentos
            .Where(a => a.UserId == userId && a.Horario.Date == agendamentoDto.Horario.Date)
            .FirstOrDefaultAsync();

        if (existingAgendamentoForUser != null)
        {
            throw new InvalidOperationException("O usuário já possui um agendamento para este dia.");
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException("Usuário não encontrado.");
        }

        var agendamento = new Agendamento
        {
            Nome = user.UserName,
            TipoCorte = agendamentoDto.TipoCorte,
            Preco = GetPrecoByTipoCorte(agendamentoDto.TipoCorte),
            Horario = agendamentoDto.Horario,
            Status = StatusAgendamento.Reservado,
            UserId = userId,
            CreatedAt = DateTime.Now
        };


        // UpdateStatus(agendamento);
        _context.Agendamentos.Add(agendamento);
        await _context.SaveChangesAsync();

        return agendamento;
    }

    public decimal GetPrecoByTipoCorte(string tipoCorte)
    {
        return tipoCorte switch
        {
            "Disfarçado" => 25.00m,
            "Corte com barba" => 40.00m,
            "Barba" => 25.00m,
            _ => 20.00m
        };
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

    // public async Task<Agendamento> ReserveAsync(int id, int userId)
    // {
    //     var agendamento = await _context.Agendamentos.FindAsync(id);
    //     if (agendamento == null || agendamento.Status != StatusAgendamento.Disponivel) return null;

    //     agendamento.Status = StatusAgendamento.Reservado;
    //     agendamento.UserId = userId;
    //     agendamento.UpdatedAt = DateTime.Now;

    //     _context.Agendamentos.Update(agendamento);
    //     await _context.SaveChangesAsync();
    //     return agendamento;
    // }

    public async Task<Agendamento> CancelAsync(int id)
    {
        var agendamento = await _context.Agendamentos.FindAsync(id);
        if (agendamento == null) return null;

        agendamento.Status = DateTime.Now > agendamento.Horario ? StatusAgendamento.Cancelado : StatusAgendamento.Disponivel;
        agendamento.UserId = null;

        UpdateStatus(agendamento);
        _context.Agendamentos.Update(agendamento);
        await _context.SaveChangesAsync();
        return agendamento;
    }

    public void UpdateStatus(Agendamento agendamento)
    {
        var now = DateTime.Now;
        var timeSpanUntilStart = agendamento.Horario.Subtract(now);
        var timeSpanUntilEnd = agendamento.Horario.AddMinutes(30).Subtract(now);

        if (agendamento.Status == StatusAgendamento.Reservado && timeSpanUntilStart.TotalMinutes <= 0 && timeSpanUntilEnd.TotalMinutes >= 0)
        {
            agendamento.Status = StatusAgendamento.Andamento;
        }
        else if (agendamento.Status == StatusAgendamento.Andamento && timeSpanUntilEnd.TotalMinutes < 0)
        {
            agendamento.Status = StatusAgendamento.Cancelado;
        }

        _context.Agendamentos.Update(agendamento);
        _context.SaveChanges();
    }

    // Task<List<Agendamento>> IAgendamentoService.GetAllAsync2()
    // {
    //     throw new NotImplementedException();
    // }
}
