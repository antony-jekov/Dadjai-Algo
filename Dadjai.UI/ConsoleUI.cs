using Dadjai.Common;
using Dadjai.WebSpider;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Dadjai.UI
{
    internal class ConsoleUI
    {
        private static void Main(string[] args)
        {
            string rootAddress = "http://academy.telerik.com/";
            //string rootAddress = "http://telerikacademy.com/";
            //string rootAddress = "http://konkurs.pcmagbg.net/";
            Encoding encode = Encoding.UTF8;
            Console.WindowWidth = Console.BufferWidth = 120;
            Console.WindowHeight = 30;
            Console.BufferHeight = 1000;
            Console.InputEncoding = encode;
            Spider spider = new Spider(rootAddress, encode);
            spider.RestoreRedirectsDB(FileManager.GetData("Redirects.dat", encode));
            DownloadedData data = spider.GetData();

            StoreAllData(data, encode);
        }

        // STORE ALL THE DOWNLOADED DATA
        private static void StoreAllData(DownloadedData data, Encoding encode)
        {
            FileManager.StoreData("PagesNews.dat", data.GetPages(), encode);
            var links = data.GetLinks();
            List<string> linksSeparated = new List<string>(links.Count() * 3);

            foreach (string link in links)
            {
                string[] separatedLinkParts = link.Split('|');
                linksSeparated.Add(separatedLinkParts[0]);
                linksSeparated.Add(separatedLinkParts[1]);
                linksSeparated.Add(separatedLinkParts[2]);
            }

            FileManager.StoreData("LinksNews.dat", linksSeparated.ToArray(), encode);
            FileManager.StoreData("ErrorsNews.dat", data.GetErrors(), encode);
            FileManager.StoreData("RedirectsNews.dat", data.GetRedirects(), encode);
            FileManager.StoreData("UnfinishedNews.dat", data.GetUnfinnished(), encode);
        }
    }
}