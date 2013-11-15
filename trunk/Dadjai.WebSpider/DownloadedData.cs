using System.Collections.Generic;

namespace Dadjai.WebSpider
{
    public class DownloadedData
    {
        private HashSet<string> errors = new HashSet<string>();
        private HashSet<string> allPages = new HashSet<string>();
        private HashSet<string> allLinks = new HashSet<string>();
        private HashSet<string> unfinnished = new HashSet<string>();
        private Dictionary<string, string> redirects = new Dictionary<string, string>();

        public IEnumerable<string> GetPages()
        {
            return this.allPages;
        }

        public IEnumerable<string> GetLinks()
        {
            return this.allLinks;
        }

        public IEnumerable<string> GetErrors()
        {
            return this.errors;
        }

        public IEnumerable<string> GetUnfinnished()
        {
            return this.unfinnished;
        }

        public void AddErrorMsg(string errorMsg)
        {
            if (!errors.Contains(errorMsg))
            {
                errors.Add(errorMsg);
            }
        }

        public void AddUnfinished(string pageAddress)
        {
            if (!unfinnished.Contains(pageAddress))
            {
                unfinnished.Add(pageAddress);
            }
        }

        public bool AddPage(string pageAddress)
        {
            if (!allPages.Contains(pageAddress))
            {
                allPages.Add(pageAddress);
                return true;
            }

            return false;
        }

        public bool AddLink(string from, string to, string label)
        {
            string link = string.Format("{0}|{1}|{2}", from, to, label);
            if (!allLinks.Contains(link))
            {
                allLinks.Add(link);
                return true;
            }
            
            return false;
        }

        public bool KnownRedirect(string addressFrom)
        {
            if (redirects.ContainsKey(addressFrom))
            {
                return true;
            }

            return false;
        }

        public string GetRedirect(string addressFrom)
        {
            return redirects[addressFrom];
        }

        public IEnumerable<string> GetRedirects()
        {
            List<string> redirectsMerged = new List<string>();

            foreach (var item in redirects)
            {
                redirectsMerged.Add(item.Key);
                redirectsMerged.Add(item.Value);
            }

            return redirectsMerged;
        }

        public void AddRedirect(string from, string to)
        {
            redirects.Add(from, to);
        }
    }
}