using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AngularBBS.Models.GithubViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string code { get; set; }

        [Required]
        public string redirect_uri { get; set; }

        public string state { get; set; }
    }
}
