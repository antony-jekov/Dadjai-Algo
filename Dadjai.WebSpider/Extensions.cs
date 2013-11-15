using System.Collections.Generic;

namespace Dadjai.WebSpider
{
    public static class Extensions
    {
        // ADDS MISSING PARTS FROM THE LINK'S BODY SO THAT IT BECOMES IT'S DESTINATION ADDRESS
        internal static string CompleteLinkBody(this string link, string pageCurrentAddress)
        {
            string linkCurrent = link;
            string basePath = pageCurrentAddress;

            if (linkCurrent.StartsWith("/"))
            {
                linkCurrent = linkCurrent.Substring(1);
                if (basePath.Contains(".com"))
                {
                    basePath = basePath.Substring(0, basePath.IndexOf(".com") + 4);
                }
                else
                {
                    basePath = basePath.Substring(0, basePath.IndexOf(".net") + 4);
                }
            }

            if (!linkCurrent.StartsWith("/") && !linkCurrent.Contains("../") && !linkCurrent.Contains("http") && !basePath.EndsWith("/"))
            {
                linkCurrent = "/" + linkCurrent;
            }

            if (!link.Contains("http"))
            {
                while (linkCurrent.Contains(".."))
                {
                    int baseIndexFarEnd = basePath.LastIndexOf("/");
                    int baseIndexEnd = basePath.LastIndexOf("/", baseIndexFarEnd - 2);
                    basePath = basePath.Substring(0, baseIndexEnd + 1);
                    linkCurrent = linkCurrent.Substring(3);
                }
                linkCurrent = basePath + linkCurrent;
            }

            while (linkCurrent.Contains("/./"))
            {
                int index = linkCurrent.IndexOf("/./");
                linkCurrent = linkCurrent.Remove(index, 2);
            }

            return linkCurrent;
        }

        // MAKES SURE NO ENDLES LOOPS OCCURE SUCH AS VIDEO/VIDEO/VIDEO/VIDEO
        internal static bool CheckForLoops(this string linkCurrent)
        {
            bool gtg = true;
            string tmpLink = linkCurrent;
            tmpLink = tmpLink.Substring(tmpLink.IndexOf("//") + 2);

            int count;
            string[] linkParts = tmpLink.Split('/');

            foreach (string part1 in linkParts)
            {
                count = 0;
                foreach (string part2 in linkParts)
                {
                    if (part1 == part2)
                    {
                        count++;
                    }
                }

                if (linkCurrent.StartsWith("http://telerik"))
                {
                    if (count > 2)
                    {
                        gtg = false;
                        break;
                    }
                }
                else if (count > 1)
                {
                    gtg = false;
                    break;
                }
            }
            return gtg;
        }

        // PARESES ALL LINKS FROM THE GIVEN HTML CODE

        internal static string DeTagLink(this string link)
        {
            int index = link.LastIndexOf("/", link.IndexOf("#")) + 1;
            return link.Substring(0, index);
        }

        internal static string[] GetLinks(this string pageContent, string currentAddress)
        {
            List<string> links = new List<string>();
            int lastStart = 0;
            int position = 0;

            // GET RID OF FORBIDDEN SECTIONS OF THE CODE (FOLLOWING COMPETIOTION RULES)
            if (currentAddress == "http://telerikacademy.com/")
            {
                int start = pageContent.IndexOf("InnerBlocsSortableList");
                int end = pageContent.IndexOf("</ul>", start);
                pageContent = pageContent.Remove(start, end - start);
            }
            else if (currentAddress.StartsWith("http://www.youtube.com/watch?"))
            {
                int start = pageContent.IndexOf("eow-description");
                int end = pageContent.IndexOf("</p>", start);
                pageContent = pageContent.Substring(start, end - start);
            }
            else if (currentAddress.StartsWith("http://www.youtube.com/playlist?"))
            {
                int start = pageContent.IndexOf("primary-pane");
                int end = pageContent.IndexOf("secondary-pane", start);
                pageContent = pageContent.Substring(start, end - start);
            }

            if (!pageContent.Contains("<a ") || !pageContent.Contains("href"))
            {
                return new string[] { };
            }
            while (lastStart < pageContent.Length)
            {
                position = pageContent.IndexOf("<a ", lastStart);
                if (position != -1)
                {
                    lastStart = position;
                    int end = pageContent.IndexOf("</a>", lastStart);
                    string tag = pageContent.Substring(lastStart, end - lastStart);
                    // MAKE SURE THE TAG FOLLOWS THE RULES
                    if (!tag.Contains("comment-") && tag.Contains("href") && !tag.Contains("currentNode"))
                    {
                        int positionHref = 0;
                        int linkPosition = 0;
                        int linkLen = 0;
                        string link = string.Empty;

                        try
                        {
                            positionHref = tag.IndexOf("href");
                            linkPosition = tag.IndexOf("\"", positionHref) + 1;
                            linkLen = tag.IndexOf("\"", linkPosition) - linkPosition;
                            link = tag.Substring(linkPosition, linkLen);
                        }
                        catch // IN CASE SINGLE QUOTES HAVE BEEN USED - WHO USES THEM - I KNOW!
                        {
                            positionHref = tag.IndexOf("href");
                            linkPosition = tag.IndexOf("\'", positionHref) + 1;
                            linkLen = tag.IndexOf("\'", linkPosition) - linkPosition;
                            link = tag.Substring(linkPosition, linkLen);
                        }

                        link = tag.Substring(linkPosition, linkLen);

                        //  || link.Contains(".pdf")
                        //  || link.Contains(".pptx") || link.Contains(".png") || link.Contains(".zip")
                        //  || link.Contains(".rar") || link.Contains(".doc") || link.Contains(".docx")
                        if (link == "" || link.Contains("javascript:") || link.Contains("()")
                            || link.Contains("mailto:") || link == ("#")
                            || link.Contains(".pdf")
                            || link.Contains(".pptx") || link.Contains(".png") || link.Contains(".zip")
                            || link.Contains(".rar") || link.Contains(".doc") || link.Contains(".docx")
                            || link.Contains("?utm")
                            //|| link.Contains("?tagName")
                            || link.Contains("?sv")
                            )
                        {
                            goto skipper;
                        }

                        link = link.TrimEnd(new char[] { '\r', '\n', ' ' });

                        if (!links.Contains(link))
                        {
                            links.Add(link);
                        }
                    }
                // INCREASE BUT DON'T ADD LINK
                skipper:
                    lastStart += end - lastStart;
                }
                else
                {
                    break;
                }
            }
            return links.ToArray();
        }
    }
}