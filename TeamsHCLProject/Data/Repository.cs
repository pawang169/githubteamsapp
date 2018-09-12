using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamsHCLProject.Data
{
    public class Repository
    {
        public string id { get; set; }
        public string name { get; set; }
        public object homepageUrl { get; set; }
        public string resourcePath { get; set; }
        public bool isPrivate { get; set; }
        public DateTime updatedAt { get; set; }
        public DateTime createdAt { get; set; }
        public string nameWithOwner { get; set; }
        public string url { get; set; }
        public Owner owner { get; set; }
    }

    public class Data
    {
        public Repository repository { get; set; }
        public Viewer viewer { get; set; }
    }

    public class RepositoryRoot
    {
        public Data data { get; set; }
    }
}