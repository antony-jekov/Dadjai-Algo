using Dadjai.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dadjai.JSTranslator
{
    internal class JSTranslatorUI
    {
        private static void Main(string[] args)
        {
            string[] allPages = FileManager.GetData("Pages.dat", Encoding.UTF8).ToArray();
            string[] allLinks = FileManager.GetData("Links.dat", Encoding.UTF8).ToArray();

            Dictionary<string, Page> pages = new Dictionary<string, Page>();
            List<Link> links = new List<Link>();

            int count = 0;
            for (int i = 0; i < allPages.Length; )
            {
                string currentAddress = allPages[i++];
                //if (!allPages.Contains(currentAddress.Substring(0, currentAddress.Length - 1)))
                {
                    pages.Add(currentAddress, new Page(currentAddress));
                    Console.WriteLine(count++);
                }
            }
            count = 0;

            for (int i = 0; i < allLinks.Length; )
            {
                string from = allLinks[i++];
                string to = allLinks[i++]; ;
                string label = allLinks[i++];
                try
                {
                    links.Add(new Link(pages[from], pages[to], label));
                }
                catch (Exception)
                {
                    Console.WriteLine("SKIPPED LINK!!!");
                }
            }

            StringBuilder sb = new StringBuilder();
            List<string> pagesForSort = new List<string>();
            foreach (var item in pages)
            {
                pagesForSort.Add(item.Key);
            }

            string[] pagesSorted = pagesForSort.ToArray();
            Array.Sort(pagesSorted);

            sb.AppendLine("var pagesDropDown = [");
            for (int i = 0; i < pagesSorted.Length; i++)
            {
                sb.AppendLine(string.Format("'{0}',", pagesSorted[i]));
            }
            sb.AppendLine("]");
            File.WriteAllText("allPagesDropDown.js", sb.ToString());

            List<string> linksFiltered = new List<string>();
            foreach (var item in links)
            {
                linksFiltered.Add(item.FromPage.Address);
                linksFiltered.Add(item.ToPage.Address);
                linksFiltered.Add(item.Label);
            }

            FileManager.StoreData("PagesFiltered.dat", pagesSorted, Encoding.UTF8);
            FileManager.StoreData("LinksFiltered.dat", linksFiltered.ToArray(), Encoding.UTF8);
        }
    }
}