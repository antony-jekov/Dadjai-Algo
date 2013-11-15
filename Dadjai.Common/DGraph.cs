using System.Collections.Generic;

namespace Dadjai.Common
{
    // DIRECTED GRAPH WITH WEIGHTLESS EDGES
    public class DGraph
    {
        protected Dictionary<string, Page> graphAllPages;

        public DGraph()
        {
            graphAllPages = new Dictionary<string, Page>();
        }

        public void AddPage(string pageAddress)
        {
            graphAllPages.Add(pageAddress, new Page(pageAddress));
        }

        public void AddLink(string from, string to, string linkLabel)
        {
            Page fromPage = graphAllPages[from];
            Page toPage = graphAllPages[to];
            fromPage.Connections.Add(new Link(fromPage, toPage, linkLabel));
        }

        public void ObliviatePages()
        {
            foreach (var page in graphAllPages)
            {
                page.Value.Discoverer = string.Empty;
                page.Value.State = PageState.Unvisited;
            }
        }

        // BFS FOR FINDING THE SHORTEST PATH BETWEEN PAGES
        public string FindShortestPath(string rootPageStr, string destinationStr)
        {
            string path = string.Empty;
            Page rootPage = graphAllPages[rootPageStr];
            rootPage.State = PageState.Visited;
            Queue<Page> queue = new Queue<Page>();

            queue.Enqueue(rootPage);
            while (queue.Count != 0)
            {
                Page temp = queue.Dequeue();
                foreach (Link Link in temp.Connections)
                {
                    if (Link.ToPage.State == PageState.Unvisited)
                    {
                        Link.ToPage.State = PageState.Visited;
                        Link.ToPage.Discoverer = temp.Discoverer + Link.Label + "|";
                        queue.Enqueue(Link.ToPage);
                    }
                }
                temp.State = PageState.Processed;
                if (destinationStr == temp.Address)
                {
                    string pathStr = temp.Discoverer;
                    path = pathStr.TrimEnd('|').Replace('|', '\n');
                    break;
                }
            }
            return path;
        }
    }
}