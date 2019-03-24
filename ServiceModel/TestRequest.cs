using ServiceStack;
using System;

namespace ServiceModel
{
    [Route("/Test/Tests", "GET")]
    [Route("/Test/Tests", "POST")]
    [Route("/Test/Tests", "PUT")]
    [Route("/Test/Tests/{id}", "DELETE")]
    public class TestRequest : IReturn<object>
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}
