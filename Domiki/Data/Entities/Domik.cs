using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domiki.Data
{
    [Table("Domik")]
    public class Domik
    {
        [Key]
        [Column(Order = 1)]
        public int Id { get; set; }

        [Key]
        [Column(Order = 2)]
        public int PlayerId { get; set; }

        public int TypeId { get; set; }

        public int Level { get; set; }
    }
}