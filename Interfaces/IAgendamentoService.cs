using sistema_barbearia.DTOs;
using sistema_barbearia.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IAgendamentoService
{
    Task<List<Agendamento>> GetAllAsync();
    Task<Agendamento> GetByIdAsync(int id);
    Task<Agendamento> CreateAsync(CreateAgendamentoDto agendamentoDto, int userId);
    Task<Agendamento> UpdateAsync(int id, Agendamento agendamento);
    Task<bool> DeleteAsync(int id);
    // Task<Agendamento> ReserveAsync(int id, int userId);
    Task<Agendamento> CancelAsync(int id);
    void UpdateStatus(Agendamento agendamento);
    decimal GetPrecoByTipoCorte(string tipoCorte);
}
