using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlojPodataka.Model
{
    [Table("predmet")]
    public class PredmetModel
    {
        [Key]
        [Column(TypeName = "char(5)")]
        [StringLength(5)]
        public string ID { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string NazivPredmeta { get; set; } = string.Empty;

    }
}
