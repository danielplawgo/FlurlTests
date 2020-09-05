using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl;
using Flurl.Http;
using Flurl.Http.Testing;

namespace FlurlTests
{
    class Program
    {
        static async Task Main(string[] args)
        {
            BuildUrl();
            await GetData();
            await TestGetData();
        }

        private static void BuildUrl()
        {
            var url = "https://flurltestservice.azurewebsites.net/"
                .AppendPathSegment("v2")
                .AppendPathSegment("users")
                .SetQueryParams(new
                {
                    AccessToken = "token",
                    ResultsCounts = 20,
                    q = "Tomasz Żyrek"
                })
                .SetFragment("Result");

            Console.WriteLine(url);
        }

        private static async Task<string> GetData()
        {
            var result = await "https://flurltestservice.azurewebsites.net/"
                .AppendPathSegment("v2")
                .AppendPathSegment("users")
                .SetQueryParams(new
                {
                    AccessToken = "token",
                    ResultsCounts = 20,
                    q = "Tomasz Żyrek"
                })
                .SetFragment("Results")
                .GetAsync()
                .ReceiveString();

            return result;
        }

        public static async Task TestGetData()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith("json result", 200);
                
                var result = await GetData();

                //domyślny assert
                httpTest.ShouldHaveCalled("https://flurltestservice.azurewebsites.net/v2/users?AccessToken=token&ResultsCounts=20&q=Tomasz%20%C5%BByrek#Results")
                    .WithVerb(HttpMethod.Get)
                    .Times(1);

                //problematyczny assert
                //httpTest.ShouldHaveCalled("https://flurltestservice.azurewebsites.net/v2/users?AccessToken=token&ResultsCounts=20&q=Tomasz")
                //    .WithVerb(HttpMethod.Get)
                //    .Times(1);

                //poprawiony assert
                //httpTest.ShouldHaveExactCall("https://flurltestservice.azurewebsites.net/v2/users?AccessToken=token&ResultsCounts=20&q=Tomasz")
                //    .WithVerb(HttpMethod.Get)
                //    .Times(1);

                result
                    .Should()
                    .Be("json result");

                Console.WriteLine("Test completed.");
            }
        }
    }

    public static class HttpCallAssertionExtensions
    {
        public static HttpCallAssertion ShouldHaveExactCall(this HttpTest test, string exactUrl)
        {
            test.CallLog.First().FlurlRequest.Url.ToString().Should().Be(exactUrl);
            return new HttpCallAssertion(test.CallLog);
        }
    }
}
