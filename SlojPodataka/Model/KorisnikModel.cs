using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlojPodataka.Model
{

    [Table("Korisnik")]
    public class KorisnikModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [StringLength(30)]
        public string Ime { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string Prezime { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string KorisnickoIme { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public Guid PasswordSalt { get; set; }

        [Required]
        [Column(TypeName = "binary(64)")]
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

        [Required]
        public int Uloga { get; set; }

    }
}
