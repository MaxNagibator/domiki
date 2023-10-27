using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domiki.Data
{
    [Table("Players")]
    public class Player
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        [MaxLength(450)]
        [Required(AllowEmptyStrings = false)]
        public string AspNetUserId { get; set; }
    }
}