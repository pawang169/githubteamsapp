using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamsHCLProject.Data
{
    //public class Node
    //{
    //    public string name { get; set; }
    //}

    public class Repositories
    {
        public List<Node> nodes { get; set; }
    }

    public class RepositoryViewer
    {
        public object name { get; set; }
        public Repositories repositories { get; set; }
    }

    public class RepositoryData
    {
        public RepositoryViewer viewer { get; set; }
    }

    public class AllRepository
    {
        public RepositoryData data { get; set; }
    }
}