using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlojPodataka.Model
{
    [Table("Ucenik")]
    public class UcenikModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [StringLength(5)]
        [MinLength(5, ErrorMessage = "Sifra ucenika mora imati tacno 5 karaktera.")]
        public string SifraUcenika { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string Ime { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string Prezime { get; set; } = string.Empty;

        [Required]
        public int BrojBodova { get; set; }

        [Required]
        [ForeignKey(nameof(TakmicenjeModel))]
        public int IDTakmicenja { get; set; }
    }
}
