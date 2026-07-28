using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlojPodataka.Model
{
    [Table("Diploma")]
    public class DiplomaModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [ForeignKey(nameof(UcenikModel))]
        public int IDUcenika { get; set; }

        [Required]
        public int Nagrada { get; set; }
    }
}
