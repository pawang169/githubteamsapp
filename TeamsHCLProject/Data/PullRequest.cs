﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamsHCLProject.Data
{
    public class Owner
    {
        public string __typename { get; set; }
        public string resourcePath { get; set; }
    }

    //public class Repository
    //{
    //    public string id { get; set; }
    //    public string name { get; set; }
    //    public string nameWithOwner { get; set; }
    //    public string resourcePath { get; set; }
    //    public string url { get; set; }
    //    public Owner owner { get; set; }
    //}

    public class Assignees
    {
        public int totalCount { get; set; }
    }

    public class Comments
    {
        public int totalCount { get; set; }
    }

    public class HeadRef
    {
        public string name { get; set; }
    }

    public class Node
    {
        public string id { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public string state { get; set; }
        public string headRefName { get; set; }
        public string revertUrl { get; set; }
        public string url { get; set; }
        public string bodyText { get; set; }
        public Repository repository { get; set; }
        public Assignees assignees { get; set; }
        public Comments comments { get; set; }
        public HeadRef headRef { get; set; }
    }

    public class Edge
    {
        public Node node { get; set; }
    }

    public class PullRequests
    {
        public int totalCount { get; set; }
        public List<Edge> edges { get; set; }
    }

    public class Viewer
    {
        public PullRequests pullRequests { get; set; }


    }

    //public class PullRequestData
    //{
    //    public Viewer viewer { get; set; }
    //}

    public class RootPullRequest
    {
        public Data data { get; set; }
    }
}