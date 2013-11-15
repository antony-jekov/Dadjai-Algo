using System.Collections.Generic;

namespace Dadjai.Common
{
    public class Page
    {
        private List<Link> connections;

        public int Number { get; set; }

        public string Address { get; set; }

        public string Label { get; set; }

        public string Discoverer { get; set; }

        public PageState State { get; set; }

        public int ID { get; private set; }

        public List<Link> Connections { get { return this.connections; } }

        public Page(int id)
        {
            connections = new List<Link>();
            State = PageState.Unvisited;
        }

        public Page(string address)
        {
            Address = address;
            State = PageState.Unvisited;
            connections = new List<Link>();
        }
    }
}