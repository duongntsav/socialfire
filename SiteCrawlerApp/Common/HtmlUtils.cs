using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace SiteCrawlerApp.Common
{
    public class HtmlUtils
    {

        // recuisive node to remove all attribute
        public static void removeAllAttribute(HtmlNode node, string exceptNode = null)
        {
            // in common case
            // List<string> exceptList = new List<string>( exceptNode.Split(";"));
            List<HtmlNode> foundNodeList = new List<HtmlNode>();

            node.Attributes.Remove();
            if (node.ChildNodes.Count > 0)
                foreach (var eachNode in getAllChild( node))
                {
                    // except img
                    if (!eachNode.Name.Equals(exceptNode))
                        eachNode.Attributes.Remove();
                }
        }

        public static string RemoveRedundanceContent(HtmlNode parentNode, List<string> textList)
        {
            string response = "";
            CultureInfo vietnam = new CultureInfo("vi-VN");
            StringComparer stringCompare = StringComparer.Create(vietnam, true);

            // 1. Crop unused tag (move to step before)
            //if (cropable)
            // parentNode.SelectNodes("//style|//script|//iframe|//caption").ToList().ForEach(n => n.Remove());

            // 2. If content have title and description. remove it and parent empty
            List<HtmlNode> foundNodeList = new List<HtmlNode>();
            List<HtmlNode> NodeList = new List<HtmlNode>();
            foreach (string text in textList)
            {
                // 1. Remove title, description if exist
                // avoid caption of news like this TNO, DT, TTO,...
                if (string.IsNullOrEmpty(text) || (!string.IsNullOrEmpty(text) && text.Length > Constants.MINLENGTH_DESCRIPTION))
                    foreach (HtmlNode node in getAllChild(parentNode))
                    {
                        // stringCompare
                        // if (node.LastChild == null && string.Compare( node.InnerText.Trim(), text) ==0)
                        if (node.ChildNodes.Count==0 && stringCompare.Compare(node.InnerText.Trim(), text) == 0)
                        {
                            // expcep image here, to process img later
                            if (node.Name.ToLower().Equals("img"))
                                continue;

                            HtmlNode foundNode = findAncestorNode(node, text.Trim());
                            if( !foundNode.InnerHtml.ToLower().Contains("img"))
                                foundNodeList.Add(foundNode);
                        }
                    }
            }

            // Loop to set empty, set here to collepse parent tag
            foreach (HtmlNode node in foundNodeList)
                node.InnerHtml = "";
            // node.remove();


            // 3. Remove all attribute except image 
            HtmlUtils.removeAllAttribute(parentNode, "img");

            // 4. remove image haved bad src
            List<HtmlNode> foundNodeList2 = new List<HtmlNode>();
            if (parentNode.ChildNodes.Count > 0)
                foreach (var eachNode in getAllChild(parentNode))
                {
                    // only img without empty src
                    if (!eachNode.Name.ToLower().Equals("img"))
                        continue;

                    string src = eachNode.Attributes["src"]?.Value;
                    if (string.IsNullOrEmpty(src) || !(src.StartsWith("http") || (src.StartsWith("image/")))) {
                        // If src removed imge tag is can not find
                        HtmlNode foundNode = findAncestorNode(eachNode, "");
                        foundNodeList2.Add(foundNode);
                    }
                }

            // Loop to set empty
            foreach (HtmlNode node in foundNodeList2)
                node.Remove();

            // 5. Remove hyperlink
            response = parentNode.InnerHtml;
            response = Regex.Replace(response, "</?(a|A).*?>", "");

            return response;
        }

        protected static HtmlNode findAncestorNode(HtmlNode node, string compareText)
        {
            // IF same to find parent
            if (node.InnerText.Trim().Equals(compareText))
            {
                if (node.ParentNode == null)
                    return node;
                else
                {
                    if (node.ParentNode.InnerText.Trim().Equals(compareText))
                        return findAncestorNode(node.ParentNode, compareText);
                    else 
                        return node;
                }
            } else
                return node;
        }

        public static List<HtmlNode> getAllChild(HtmlNode node)
        {
            List<HtmlNode> childList = new List<HtmlNode>();

            foreach (HtmlNode n in node.ChildNodes)
            {
                if (n.HasChildNodes)
                {
                    List<HtmlNode> childList2 = getAllChild(n);
                    childList.AddRange(childList2);
                }
                else
                {
                    childList.Add(n);
                }
            }
            return childList;
        }


        public static bool checkRedirectURL(HtmlAgilityPack.HtmlDocument doc)
        {
            var xpath = "//meta[@http-equiv='refresh' and contains(@content, 'URL')]";
            var refresh = doc.DocumentNode.SelectSingleNode(xpath);
            if (refresh == null)
                return false;
            var content = refresh.Attributes["content"].Value;
            string urlRidirect= Regex.Match(content, @"\s*URL\s*=\s*([^ ;]+)").Groups[1].Value.Trim();
            if (!string.IsNullOrEmpty(urlRidirect))
                return true;

            return false;
        }

        public static string GetMetaRefreshUrl(string sourceUrl)
        {
            var web = new HtmlWeb();
            var doc = web.Load(sourceUrl);
            var xpath = "//meta[@http-equiv='refresh' and contains(@content, 'URL')]";
            var refresh = doc.DocumentNode.SelectSingleNode(xpath);
            if (refresh == null)
                return null;
            var content = refresh.Attributes["content"].Value;
            return Regex.Match(content, @"\s*URL\s*=\s*([^ ;]+)").Groups[1].Value.Trim();
        }

        public static string GetFinalRedirect(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return url;

            int maxRedirCount = 8;  // prevent infinite loops
            string newUrl = url;
            do
            {
                HttpWebRequest req = null;
                HttpWebResponse resp = null;
                try
                {
                    req = (HttpWebRequest)HttpWebRequest.Create(url);
                    req.Method = "HEAD";
                    req.AllowAutoRedirect = false;
                    resp = (HttpWebResponse)req.GetResponse();
                    switch (resp.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            return newUrl;
                        case HttpStatusCode.Redirect:
                        case HttpStatusCode.MovedPermanently:
                        case HttpStatusCode.RedirectKeepVerb:
                        case HttpStatusCode.RedirectMethod:
                            newUrl = resp.Headers["Location"];
                            if (newUrl == null)
                                return url;

                            if (newUrl.IndexOf("://", System.StringComparison.Ordinal) == -1)
                            {
                                // Doesn't have a URL Schema, meaning it's a relative or absolute URL
                                Uri u = new Uri(new Uri(url), newUrl);
                                newUrl = u.ToString();
                            }
                            break;
                        default:
                            return newUrl;
                    }
                    url = newUrl;
                }
                catch (WebException)
                {
                    // Return the last known good URL
                    return newUrl;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    if (resp != null)
                        resp.Close();
                }
            } while (maxRedirCount-- > 0);

            return newUrl;
        }
    }
}
