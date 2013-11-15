using Dadjai.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Dadjai.Oracle
{
    internal class OracleUI
    {
        private static void Main(string[] args)
        {
            HashSet<string> pagesAll = new HashSet<string>();

            //DateTime start = DateTime.UtcNow;
            Encoding encoding = Encoding.UTF8;
            var pages = FileManager.GetData("Pages.dat", encoding);
            string[] links = FileManager.GetData("Links.dat", encoding).ToArray();
            DGraph graph = new DGraph();

            foreach (var page in pages)
            {
                graph.AddPage(page);
                pagesAll.Add(page);
            }

            for (int l = 0, len = links.Length; l < len; )
            {
                graph.AddLink(links[l++], links[l++], links[l++]);
            }

            //string from = "http://academy.telerik.com/student-courses/archive/web-design-html-5-css-3-javascript/homework";
            //string to = "http://konkurs.pcmagbg.net/results-3-season-2012-2013-2/";
            string from = Console.ReadLine().Trim();
            string to = Console.ReadLine().Trim();

            if (!pagesAll.Contains(from))
            {
                from = GetAbsoluteAddress(from);
            }
            if (!pagesAll.Contains(to))
            {
                to = GetAbsoluteAddress(to);
            }

            try
            {
                Console.WriteLine(graph.FindShortestPath(from, to));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            //DateTime stop = DateTime.UtcNow;
            //Console.WriteLine((stop - start).TotalSeconds);
        }

        private static string GetAbsoluteAddress(string address)
        {
            WebRequest req = WebRequest.Create(address);
            WebResponse res = req.GetResponse();
            return res.ResponseUri.AbsoluteUri;
        }
    }
}