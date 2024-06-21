using sistema_barbearia.DTOs;
namespace sistema_barbearia.DTOs
{


    public class AgendamentoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string TipoCorte { get; set; }
        public decimal Preco { get; set; }
        public DateTime Horario { get; set; }
        public int Status { get; set; }
        public UserDto Usuario { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}


