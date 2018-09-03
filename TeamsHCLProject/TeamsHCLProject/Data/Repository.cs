using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamsHCLProject.Data
{
    public class Repository
    {
        public Data data { get; set; }
    }
    public class Node
    {
        public string name { get; set; }
    }

    public class Repositories
    {
        public List<Node> nodes { get; set; }
    }

    public class Viewer
    {
        public object name { get; set; }
        public Repositories repositories { get; set; }
    }

    public class Data
    {
        public Viewer viewer { get; set; }
    }
}