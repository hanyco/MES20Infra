using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Library.Data.SqlServer;

using Xunit;

using Xunit.Abstractions;

namespace InfraTestProject.Tests.Dalc;
public sealed class InsertTest(ITestOutputHelper output)
{
    private readonly ITestOutputHelper _output = output;

    [Fact]
    public void MyTestMethod()
    {
        SqlStatementBuilder.Update()
    }
}
