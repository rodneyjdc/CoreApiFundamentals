using CoreCodeCamp.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoreCodeCamp.Models
{
    public class CampModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Moniker { get; set; }

        public DateTime EventDate { get; set; } = DateTime.MinValue;

        [Range(1, 31)]
        public int Length { get; set; } = 1;

        public string Venue { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string CityTown { get; set; }
        public string StateProvince { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        public ICollection<TalkModel> Talks { get; set; }
    }
}
