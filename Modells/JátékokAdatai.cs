using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kaszinó_projekt.Modells
{
    [Table("JátékokAdatai")]
    public class JátékokAdatai
    {
        [Key]
        public int JátékId { get; set; }

        public int FelhasznaloId { get; set; }

        public string JatekTipus { get; set; } = "dice";

        public int Tet { get; set; }

        public int Nyeremeny { get; set; }

        public DateTime Datum { get; set; } = DateTime.Now;

        public int? Nyerovonalak { get; set; }

        public int EgyenlegJatekElott { get; set; }

        public int EgyenlegJatekUtan { get; set; }

        // NE LEGYEN [Required] attribútum!
        [ForeignKey("FelhasznaloId")]
        public virtual Adatokmodell? Felhasznalo { get; set; }
    }
}