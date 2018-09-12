using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamsHCLProject.Data
{
    public class TeamsSubmit
    {
        public string Action { get; set; }
        public string Repository { get; set; }
        public string IssueTitle { get; set; }
    }
}