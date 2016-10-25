using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CdnBundle.Core.Tests
{
    [TestFixture]
    public class PromiseTest
    {
        public PromiseTest()
        {

        }

        [Test]
        public void createsSuccessfully()
        {
            Assert.DoesNotThrow(() =>
            {
                Promise<bool>.Create(() =>
                {
                    return true;
                });
            });
            Console.WriteLine("Promise was created successfully");
        }

        [Test]
        public void throwsErrorIfErrorNotSpecified()
        {
            Console.WriteLine("This should throw an exception");
            try
            {
                Assert.Throws<Exception>(() =>
                {
                    Promise<bool>.Create(() =>
                    {
                        throw new Exception("Test Exception");
                    });
                });
            }
            catch
            {
                Console.WriteLine("Exception thrown successfully");
            }
        }

        [Test]
        public void waits()
        {
            Assert.That(() =>
            {
                return Promise<bool>.Create(() =>
                {
                    for (int i = 1; i <= 1000; i++)
                    {
                        Console.WriteLine(i);
                    }
                    return true;
                }).Wait().promiseStates.Contains(Promise<bool>.State.Waiting);
            });
            Console.WriteLine("This waited successfully");
        }
    }
}
