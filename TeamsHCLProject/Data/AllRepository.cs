using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamsHCLProject.Data
{
    public class AllRepository
    {
        public Data1 data { get; set; }
    }
    public class Node1
    {
        public string name { get; set; }
    }

    public class Repositories
    {
        public List<Node1> nodes { get; set; }
    }

    public class Viewer
    {
        public object name { get; set; }
        public Repositories repositories { get; set; }
    }

    public class Data1
    {
        public Viewer viewer { get; set; }
    }
}