using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamsHCLProject.Data
{
    public class Node
    {
        public string title { get; set; }
        public string url { get; set; }
        public string state { get; set; }
    }

    public class Edge
    {
        public List<Node> node { get; set; }
    }

    public class Issues
    {
        public List<Edge> edges { get; set; }
    }

    public class Repository
    {
        public Issues issues { get; set; }

        public string id { get; set; }
        public string name { get; set; }
        public object homepageUrl { get; set; }
        public string resourcePath { get; set; }
        public bool isPrivate { get; set; }
        public DateTime updatedAt { get; set; }
        public DateTime createdAt { get; set; }
        public string nameWithOwner { get; set; }
        public string url { get; set; }
      //  public Owner owner { get; set; }
    }

    public class Data
    {
        public Repository repository { get; set; }

        public ViewerPullRequest viewer { get; set; }
    }

    public class RepositoryRoot
    {
        public Data data { get; set; }
    }
}