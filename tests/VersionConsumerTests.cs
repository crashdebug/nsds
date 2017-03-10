using System;
using Xunit;
using NSDS.Core;
using NSDS.Core.Interfaces;

namespace NSDS.Tests
{
    public class VersionConsumerTests
    {
        [Fact]
        public void CartVersionTest()
        {
            var factory = new ConnectionFactory();
            factory.Add("http", new TestConnection());
        }
    }

    public class TestConnection : IConnection
    {

    }
}
