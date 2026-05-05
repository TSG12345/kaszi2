using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kaszinó_projekt.Modells
{
    [Table("KaszinóBevitel")]
    public class Kaszinobevitel
    {
        [Key]
        public int TranzakcióId { get; set; }
        public int FelhasználóId { get; set; }
        public DateTime Dátum { get; set; }
        public int Bevitel { get; set; }
        public int Kiadás { get; set; }
    }
}
