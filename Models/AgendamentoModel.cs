using System;
using sistema_barbearia.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sistema_barbearia.Models
{
    public class Agendamento
    {
        public Agendamento()
        {
            Nome = string.Empty;
            TipoCorte = string.Empty;
            CreatedAt = DateTime.Now;
            Status = StatusAgendamento.Disponivel;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public string TipoCorte { get; set; }

        [Required]
        public decimal Preco { get; set; }

        [Required]
        public DateTime Horario { get; set; }

        public StatusAgendamento Status { get; set; }

        // Foreign Key for User
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public DateTime CreatedAt { get; set; }

    }

    public enum StatusAgendamento
    {
        Disponivel,
        Reservado,
    }
}
