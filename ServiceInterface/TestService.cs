using ServiceModel;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceInterface
{
    public partial class TestService : ServiceStack.Service
    {
        [Authenticate]
        public async Task<object> Get(TestRequest request)
        {
            return new List<string>() { "param 1", "param 2", "param 3", "param 4" };

        }
    }
}
