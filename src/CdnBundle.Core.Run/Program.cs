using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CdnBundle.Core;

namespace CdnBundle.Core.Run
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Tests.PromiseTest promiseTest = new Tests.PromiseTest();
            //promiseTest.createsSuccessfully();
            //promiseTest.waits();
            //promiseTest.throwsErrorIfErrorNotSpecified();
            Console.WriteLine("End of Program!");
            Console.Read();
        }
    }
}
