﻿using CoreCodeCamp.Data;
using System.ComponentModel.DataAnnotations;

namespace CoreCodeCamp.Models
{
    public class TalkModel
    {
        public int TalkId { get; set; }
        [Required]
        [StringLength(50)]
        public string Title { get; set; }
        [Required]
        [StringLength(4000, MinimumLength = 25)]
        public string Abstract { get; set; }
        [Range(100, 500)]
        public int Level { get; set; }

        public SpeakerModel Speaker { get; set; }
    }
}
