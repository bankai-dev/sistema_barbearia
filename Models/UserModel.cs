using System.Collections.Generic;
using sistema_barbearia.Models;
using System.ComponentModel.DataAnnotations;

namespace sistema_barbearia.Models
{
    public class User
    {
        public User()
        {
            UserName = string.Empty;
            PasswordHash = Array.Empty<byte>();
            PasswordSalt = Array.Empty<byte>();
            Agendamentos = new List<Agendamento>();
        }
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public byte[] PasswordHash { get; set; }

        [Required]
        public byte[] PasswordSalt { get; set; }

        // Navigation properties
        public virtual ICollection<Agendamento> Agendamentos { get; set; }

    }
}
