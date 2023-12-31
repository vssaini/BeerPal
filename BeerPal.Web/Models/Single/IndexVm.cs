﻿using System;
using System.ComponentModel.DataAnnotations;

namespace BeerPal.Web.Models.Single
{
    public class IndexVm
    {
        public DateTime TourDate { get; set; }
        public int Price { get; set; }

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
    }
}