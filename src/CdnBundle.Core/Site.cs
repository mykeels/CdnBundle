using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace CdnBundle.Core
{
    public class Site: ControllerBase
    {
        private static IHostingEnvironment _staticEnv;
        private IHostingEnvironment _env;
        public Site(IHostingEnvironment env)
        {
            _env = env;
            _staticEnv = env;
        }

        public static HttpContext getHttpContext()
        {
            return ControllerContext.HttpContext;
        }

        public static string getRootPath()
        {
            return _staticEnv.WebRootPath;
        }
    }
}
