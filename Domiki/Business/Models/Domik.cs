﻿namespace Domiki.Web.Business.Models
{
    public class Domik
    {
        public int Id { get; set; }
        public DomikType Type { get; set; }
        public int Level { get; set; }
        public int PlayerId { get; set; }
    }
}