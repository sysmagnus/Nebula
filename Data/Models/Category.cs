﻿using System.ComponentModel.DataAnnotations;

namespace Nebula.Data.Models
{
    public class Category
    {
        public int Id { get; set; }
        [MaxLength(250)] public string Name { get; set; }
    }
}
