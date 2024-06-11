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
            UpdatedAt = DateTime.Now;
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

        [Required]
        public StatusAgendamento Status { get; set; }

        // Foreign Key for User
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        // Foreign Key for Barbeiro
        public int? BarbeiroId { get; set; }
        [ForeignKey("BarbeiroId")]
        public virtual User? Barbeiro { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public enum StatusAgendamento
    {
        Disponivel,
        Andamento,
        Reservado,
        Cancelado
    }

}
