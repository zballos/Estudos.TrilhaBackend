using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCore.Domain
{
    [Table("tbl_clientes")]
    public class Cliente
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; }
        [Column("primeiro_telefone")]
        public string Telefone { get; set; }
        public string CEP { get; set; }
        public string Estado { get; set; }
        public string Cidade { get; set; }
    }
}