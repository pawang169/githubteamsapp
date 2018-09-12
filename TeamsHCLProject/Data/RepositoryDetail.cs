using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamsHCLProject.Data
{
    public class RepositoryDetailNode
    {
        public string title { get; set; }
        public string url { get; set; }
        public string state { get; set; }
        public string createdAt { get; set; }
        public string body { get; set; }
    }

    public class RepositoryDetailEdge
    {
        public RepositoryDetailNode node { get; set; }
    }

    public class RepositoryDetailIssues
    {
        public List<RepositoryDetailEdge> edges { get; set; }
    }

    public class RepositoryDetail
    {
        public RepositoryDetailIssues issues { get; set; }
    }

    public class RepositoryDetailData
    {
        public RepositoryDetail repository { get; set; }
    }

    public class RepositoryDetailRoot
    {
        public RepositoryDetailData data { get; set; }
    }
}