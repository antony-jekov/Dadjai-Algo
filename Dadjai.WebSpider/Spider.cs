using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Dadjai.WebSpider
{
    public class Spider
    {
        // SPIDER FIELDSET
        #region
        private Queue<string> pagesQ = new Queue<string>();
        private WebClient web = new WebClient();
        private GZipBrowser gzipWeb = new GZipBrowser();
        private string rootAddress = string.Empty; // STARTING PAGE ADDRESS
        private Encoding encoding = Encoding.Default;
        private DownloadedData data = new DownloadedData(); // ALL DATA GOES HERE
        #endregion

        // PROPERTIES
        #region

        public Encoding Encoding
        {
            get { return this.encoding; }
            set { this.encoding = value; }
        }

        public string RootAddress
        {
            get { return this.rootAddress; }
            set { this.rootAddress = value; }
        }

        #endregion

        // CONSTRUCTOR
        public Spider(string rootAddress, Encoding encoding)
        {
            RootAddress = rootAddress;
            Encoding = encoding;
            web.Encoding = encoding;
            gzipWeb.Encoding = encoding;
        }

        // MAIN METHOD
        public DownloadedData GetData()
        {
            RootAddress = CheckForRedirect(RootAddress);
            int pageProcessedCount = 0;
            string[] links;
            pagesQ.Enqueue(RootAddress);
            data.AddPage(rootAddress);
            string content = string.Empty;

            // BEGIN BFS
            while (pagesQ.Count > 0)
            {
                //return data;
                string currentPage = pagesQ.Dequeue();
                pageProcessedCount++;
                #region
                Console.WriteLine();
                Console.Write(currentPage);
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(string.Format(" {0} / {1}", pageProcessedCount, pagesQ.Count));
                Console.ForegroundColor = ConsoleColor.Gray;
                #endregion
                try // ATTEMPT TO DOWNLOAD THE CURRENT PAGE AS HTML CODE
                {
                    content = ObtainCurrentPageHTMLContent(currentPage);
                }
                catch (Exception e)
                {
                    data.AddErrorMsg(string.Format("\n\n{0}\n\n", e.Message));
                    continue;
                }

                try // ATTEMPT TO PARSE ALL ANCHOR LINKS FROM THE HTML CODE
                {
                    links = content.GetLinks(currentPage);
                }
                catch // IN CASE OF FAILIURE - STORE CURRENT PAGE FOR LATTER ATTEMPT AND SKIP TO NEXT PAGE
                {
                    data.AddUnfinished(currentPage);
                    continue;
                }

                // EVALUATE EVERY LINK AND CREATE A NEW PAGE WITH THE LINK'S DESTINATION ADDRESS
                foreach (string link in links)
                {
                    // COMPLETE THE BODY OF THE LINK E.G IF THE LINK IS IN ../../LINK FORMAT
                    //pageDestinationAddress = link.CompleteLinkBody(currentPage);
                    string pageDestination = link.CompleteLinkBody(currentPage);

                    if (!pageDestination.CheckForLoops())
                    {
                        continue;
                    }

                    if (pageDestination.Contains("#"))
                    {
                        pageDestination = pageDestination.DeTagLink();
                    }
                    try // CHECK IF THE DESTINATION PAGE IS BEING REDIRECTED TO ANOTHER DESTINATION
                    {
                        //pageDestination = CheckForRedirect(pageDestination);
                    }
                    catch // IN CASE OF BROKEN OR BAD LINK - RECCORD ERROR FOR FUTURE REFFERANCE AND MARK LINK AS BROKEN
                    {
                        data.AddErrorMsg(string.Format("\n\n------------- ERROR ---------------\n{0}\n{1}\n\n", currentPage, link));
                        continue;
                    }
                    // FINAL STEP OF EVALUATION BEFORE CREATING A NEW PAGE AND MOVING TO THE NEXT LINK
                    if (EvaluateLink(pageDestination))
                    {
                        // ATTEMPTS TO ADD THE CURRENT DESTINATION TO THE QUEUE
                        if (data.AddPage(pageDestination))
                        {
                            // CONSOLE INFO DISPLAY
                            #region
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(pageDestination);
                            Console.ForegroundColor = ConsoleColor.Gray;
                            #endregion
                            // IF THE DESTINATION IS NOT A MEDIA FILE - ADD IT TO THE QUEUE
                            if (!pageDestination.Contains(".rar") && !pageDestination.Contains(".zip") && !pageDestination.Contains(".pdf")
                                && !pageDestination.Contains(".doc") && !pageDestination.Contains(".docx") && !pageDestination.Contains(".pptx")
                                && !pageDestination.Contains(".png"))
                            {
                                pagesQ.Enqueue(pageDestination);
                            }
                        }
                        // ATTEMPTS TO ADD A CONNECTING LINK FROM CURRENT PAGE TO THE DESTINATION OF THE CURRENT LINK
                        if (currentPage != pageDestination && data.AddLink(currentPage, pageDestination, link))
                        {
                            #region
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine(currentPage + "   -->   " + pageDestination);
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.WriteLine(link);
                            Console.ForegroundColor = ConsoleColor.Gray;
                            #endregion
                        }
                    }
                }
            }
            // RETURN GATHERED DATA TO CALLER
            return data;
        }

        // METHODS
        #region

        // RESTORES THE REDIRECTS DATABASE
        public void RestoreRedirectsDB(IEnumerable<string> redirects)
        {
            var redirectsArr = redirects.ToArray();
            for (int i = 0; i < redirectsArr.Length; )
            {
                data.AddRedirect(redirectsArr[i++], redirectsArr[i++]);
            }
        }

        // DOWNLOADS HTML CODE OF THE GIVEN PAGE ADDRESS
        private string ObtainCurrentPageHTMLContent(string pageCurrentAddress)
        {
            if (pageCurrentAddress.StartsWith("http://telerikacademy.com")) // MANAGES GZIPED DATA
            {
                return gzipWeb.DownloadString(pageCurrentAddress);
            }
            else
            {
                return web.DownloadString(pageCurrentAddress);
            }
        }

        // CREATES A DYNAMIC REQUEST AND COMPARES CALLING LINK WITH FINAL DESTINATION
        private string CheckForRedirect(string addressFrom)
        {
            // CONNECTS TO GATHERED DATA AND ASKS IF SUCH REDIRECT HAS ALREADY BEEN RECORDED
            if (data.KnownRedirect(addressFrom))
            {
                return data.GetRedirect(addressFrom); // RETURNS KNOWN REDIRECT
            }
            else // CREATES A NEW ONE
            {
                string finalURL = string.Empty;

                WebRequest request = WebRequest.Create(addressFrom);
                WebResponse response = request.GetResponse();
                finalURL = response.ResponseUri.AbsoluteUri;
                data.AddRedirect(addressFrom, finalURL);
                response.Close();
                return finalURL;
            }
        }

        private bool EvaluateLink(string pageDestination)
        {
            if (pageDestination.CheckForLoops() && pageDestination.Contains("http://academy.telerik.com") // || pageDestination.Contains("http://telerikacademy.com") || pageDestination.Contains("http://konkurs.pcmagbg.net") || pageDestination.Contains("http://www.youtube.com/watch?") || pageDestination.Contains("http://www.youtube.com/playlist?")
                )
            {
                return true;
            }
            return false;
        }

        #endregion
    }
}