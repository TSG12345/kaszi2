using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kaszinó_projekt.Modells
{
        [Table("FelhasználóAdatok")]
        public class Adatokmodell
        {
            [Key]
            public int Id { get; set; }
            public string Nev { get; set; }
            public string Email { get; set; }
            public string Jelszo { get; set; }
            public int Eletkor { get; set; }
            public int Egyenleg { get; set; }
            public bool Admin { get; set; }


        }
    public class LoginRequest
    {
        public string NevVagyEmail { get; set; }
        public string Jelszo { get; set; }
    }

}
