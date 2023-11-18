﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domiki.Web.Data
{
    [Table("Manufactures")]
    public class Manufacture
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int DomikId { get; set; }

        public int ResourceTypeId { get; set; }

        public int ResourceCount { get; set; }

        public int PlodderCount { get; set; }

        public Domik Domik { get; set; }

        public DateTime FinishDate { get; set; }
    }
}