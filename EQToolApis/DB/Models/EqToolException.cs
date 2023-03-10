﻿using System.ComponentModel.DataAnnotations;

namespace EQToolApis.DB.Models
{
    public class EqToolException
    {
        [Key]
        public int Id { get; set; }

        public string Exception { get; set; }

        public DateTime DateCreated { get; set; }
    }
}