using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Ajax;

namespace CdnBundle
{
    
    public class Bundle
    {
        public string cdnUrl { get; set; }
        public string localUrl { get; set; }
        public bool useMinification { get; set; }
        public BundleType type { get; set; }
        private static Dictionary<string, DateTime> cacheRecords = new Dictionary<string, DateTime>();
        private string loadType { get; set; }

        public Bundle()
        {

        }

        public Bundle(string cdnUrl, string localUrl, BundleType bundleType, bool useMinification = true)
        {
            this.cdnUrl = cdnUrl;
            this.localUrl = localUrl;
            this.type = bundleType;
            this.useMinification = useMinification;
        }

        public Bundle(string localUrl, BundleType bundleType, bool useMinification = true)
        {
            this.localUrl = localUrl;
            this.type = bundleType;
            this.useMinification = useMinification;
        }

        public enum BundleType
        {
            JavaScript,
            CSS
        }

        private static string GetLeftUrl()
        {
            string lefturl = "";
            try
            {
                if (!String.IsNullOrEmpty(lefturl)) return lefturl;
                string left = String.Format("{0}://{1}{2}/", System.Web.HttpContext.Current.Request.Url.Scheme, System.Web.HttpContext.Current.Request.Url.Authority, HttpRuntime.AppDomainAppVirtualPath);
                while (left.EndsWith("/"))
                {
                    left = left.Substring(0, left.Length - 1);
                }
                lefturl = left.ToLower() + "/";
                return lefturl;
            }
            catch (Exception ex)
            {
                
                return "/";
            }
        }

        public string getLocalFilePath()
        {
            return getLocalFilePath(localUrl);
        }

        public string getRelativePath()
        {
            return getRelativePath(localUrl);
        }

        public static string getLocalFilePath(string localUrl)
        {
            string localUrlPath = localUrl;
            if (HttpContext.Current != null && HttpContext.Current.Server != null && localUrl.StartsWith("~"))
            {
                localUrlPath = HttpContext.Current.Server.MapPath(localUrl);
            }
            else if (!String.IsNullOrEmpty(localUrlPath)) return localUrlPath.Replace("~", System.Environment.CurrentDirectory.Replace(@"\", "/"));
            return localUrlPath ?? "";
        }

        public static string getRelativePath(string localUrl)
        {
            string localUrlPath = localUrl;
            if (HttpContext.Current != null && HttpContext.Current.Server != null && localUrl.StartsWith("~"))
            {
                localUrlPath = localUrl.Replace("~/", GetLeftUrl());
            }
            return localUrlPath;
        }

        public string getLoadType()
        {
            return loadType;
        }

        public System.IO.FileInfo getLocalFileInfo()
        {
            System.IO.FileInfo file = new System.IO.FileInfo(getLocalFilePath());
            return file;
        }

        private string loadFromCdn()
        {
            loadType = "CDN";
            string response = "";
            var minifier = new Microsoft.Ajax.Utilities.Minifier();
            try
            {
                if (cdnUrl.StartsWith("~/")) cdnUrl = cdnUrl.Replace("~/", GetLeftUrl());
                response = Api.Get(cdnUrl);
                if (!cacheRecords.ContainsKey(cdnUrl) && !String.IsNullOrEmpty(response)) cacheRecords.AddSafe(cdnUrl, DateTime.Now);
                else if (String.IsNullOrEmpty(response) && System.IO.File.Exists(getLocalFilePath())) response = System.IO.File.ReadAllText(getLocalFilePath());
            }
            catch (Exception ex)
            {
                if (!String.IsNullOrEmpty(localUrl)) response = System.IO.File.ReadAllText(getLocalFilePath());
                else throw ex;
            }
            if (useMinification)
            {
                if (type == BundleType.CSS) response = minifier.MinifyStyleSheet(response);
                else response = minifier.MinifyJavaScript(response);
            }
            if (!String.IsNullOrEmpty(localUrl))
            {
                try
                {
                    response.SaveToFile(getLocalFilePath());
                }
                catch (Exception ex) { }
            }
            return response;
        }

        private string loadFromLocal()
        {
            loadType = "LOCAL";
            string fileContents = System.IO.File.ReadAllText(getLocalFilePath());
            return fileContents;
        }

        public string Load()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine(@"/**** " + (this.cdnUrl ?? this.localUrl) + " ****/");
            if (!String.IsNullOrEmpty(cdnUrl) && !cacheRecords.ContainsKey(cdnUrl))
            {
                if (!String.IsNullOrEmpty(localUrl) && System.IO.File.Exists(getLocalFilePath()))
                {
                    var file = new System.IO.FileInfo(getLocalFilePath());
                    if (DateTime.Now.Subtract(file.LastWriteTime).TotalHours <= 24)
                    {
                        sb.AppendLine(loadFromLocal());
                        sb.AppendLine();
                    }
                    else
                    {
                        sb.AppendLine(loadFromCdn());
                        sb.AppendLine();
                    }
                }
                else
                {
                    sb.AppendLine(loadFromCdn());
                    sb.AppendLine();
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(localUrl) && System.IO.File.Exists(getLocalFilePath())) //check that the file exists in file system
                {
                    sb.AppendLine(loadFromLocal());
                    sb.AppendLine();
                }
            }
            return sb.ToString();
        }

        public Promise<string> LoadAsync()
        {
            return Promise<string>.Create(() => Load());
        }
    }
}