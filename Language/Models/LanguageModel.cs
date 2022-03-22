using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Language.Models
{
    public class LanguageModel
    {
        [Required(ErrorMessage ="Key không được trống")]
        public string key { get; set; }
        [Required(ErrorMessage = "Value không được trống")]
        public string value { get; set; }
    }
}