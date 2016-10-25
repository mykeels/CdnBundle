using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Ajax;

namespace CdnBundle
{
    public static class BundleListExtensions
    {
        private static Dictionary<string, DateTime> cacheRecords = new Dictionary<string, DateTime>();
        public static void AddSafe<K, V>(this IDictionary<K, V> mydict, K key, V value)
        {
            if (mydict.ContainsKey(key))
            {
                try
                {
                    mydict[key] = value;
                }
                catch (Exception ex)
                {
                    mydict.Add(key, value);
                }
            }
            else
            {
                try
                {
                    mydict.Add(key, value);
                }
                catch (Exception ex)
                {
                    mydict[key] = value;
                }
            }
        }

        public static string Load(this IEnumerable<Bundle> bundles, string localUrl = null, bool async = false)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Bundle bundle in bundles)
            {
                sb.Append(bundle.Load());
            }
            string response = sb.ToString();
            if (!String.IsNullOrEmpty(localUrl))
            {
                if (!cacheRecords.ContainsKey(localUrl))
                {
                    cacheRecords.AddSafe(localUrl, DateTime.Now);
                }
                else if (cacheRecords.ContainsKey(localUrl) && (DateTime.Now.Subtract(cacheRecords[localUrl]).TotalHours > 24))
                {
                    cacheRecords[localUrl] = DateTime.Now;
                }
                response.SaveToFile(Bundle.getLocalFilePath(localUrl));

                if (bundles.All((b) => b.type == Bundle.BundleType.CSS))
                {
                    // css link stylesheet
                    return "<link href=\"" + Bundle.getResolvePath(localUrl) + "\" type=\"text/css\"" + (async ? " async" : "") + " rel =\"stylesheet\" />";
                }
                else
                {
                    //js script tag
                    return "<script src=\"" + Bundle.getResolvePath(localUrl) + "\" type=\"text/javascript\"" + (async ? " async" : "") + "></script>";
                }
            }
            else
            {
                if (bundles.All((b) => b.type == Bundle.BundleType.CSS)) return "<style>" + response + "</style>";
                else return "<script type=\"text/javascript\">" + response + "</script>";
            }
        }

        public static string RenderCdn(this IEnumerable<Bundle> bundles)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var bundle in bundles)
            {
                if (!String.IsNullOrEmpty(bundle.cdnUrl))
                {
                    if (bundle.type == Bundle.BundleType.CSS) sb.AppendLine("<link href=\"" + bundle.cdnUrl + "\" type=\"text/css\"" + " rel =\"stylesheet\" />");
                    else sb.AppendLine("<script src=\"" + bundle.cdnUrl + "\" type=\"text/javascript\"></script>");
                }
            }
            return sb.ToString();
        }

        public static string RenderLocal(this IEnumerable<Bundle> bundles)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var bundle in bundles)
            {
                if (!String.IsNullOrEmpty(bundle.localUrl))
                {
                    if (bundle.type == Bundle.BundleType.CSS) sb.AppendLine("<link href=\"" + bundle.getLocalFilePath() + "\" type=\"text/css\"" + " rel =\"stylesheet\" />");
                    else sb.AppendLine("<script src=\"" + bundle.getLocalFilePath() + "\" type=\"text/javascript\"></script>");
                }
            }
            return sb.ToString();
        }

        public static string Render(this IEnumerable<Bundle> bundles)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var bundle in bundles)
            {
                if (!String.IsNullOrEmpty(bundle.cdnUrl))
                {
                    if (bundle.type == Bundle.BundleType.CSS) sb.AppendLine("<link href=\"" + bundle.cdnUrl + "\" type=\"text/css\"" + " rel =\"stylesheet\" />");
                    else sb.AppendLine("<script src=\"" + bundle.cdnUrl + "\" type=\"text/javascript\"></script>");
                }
                else if (!String.IsNullOrEmpty(bundle.localUrl))
                {
                    if (bundle.type == Bundle.BundleType.CSS) sb.AppendLine("<link href=\"" + bundle.getLocalFilePath() + "\" type=\"text/css\"" + " rel =\"stylesheet\" />");
                    else sb.AppendLine("<script src=\"" + bundle.getLocalFilePath() + "\" type=\"text/javascript\"></script>");
                }
            }
            return sb.ToString();
        }

        public static void ClearAllRecords()
        {
            cacheRecords.Clear();
        }

        public static void Test()
        {
            List<Bundle> bundles = new List<Bundle>();
            bundles.Add(new Bundle("https://cdnjs.cloudflare.com/ajax/libs/jquery/3.1.0/jquery.min.js", @"~/jquery.min.js", Bundle.BundleType.JavaScript, false));
            bundles.Add(new Bundle("https://cdnjs.cloudflare.com/ajax/libs/jqueryui/1.12.0/jquery-ui.min.js", @"~/jquery-ui.min.js", Bundle.BundleType.JavaScript, false));
            bundles.Add(new Bundle(@"~/my-local-script.js", Bundle.BundleType.JavaScript, true));
        }
    }
}
