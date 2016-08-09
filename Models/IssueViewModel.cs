using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AngularBBS.Models
{
    public class IssueViewModel
    {
        [Required]
        public string Title { get; set; }
        public string Body { get; set; }

        public string Assignee { get; set; }
        public int Milestone { get; set; }
        public List<string> Labels { get; set; }

        public List<string> Assignees { get; set; }
    }
}