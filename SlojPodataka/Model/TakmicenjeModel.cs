using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlojPodataka.Model
{
    [Table("Takmicenje")]
    public class TakmicenjeModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public DateTime DatumTakmicenja { get; set; }

        [Required]
        [StringLength(5)]
        [Column(TypeName = "char(5)")]
        public string IDPredmeta { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string NazivTakmicenja { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string TipTakmicenja { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LokacijaTakmicenja { get; set; } = string.Empty;

    }
}
