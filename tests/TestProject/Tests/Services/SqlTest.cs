using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Contracts.ViewModels;

using Library.Data.SqlServer;

using Xunit;

namespace InfraTestProject.Tests.Services;
public sealed class SqlTest
{
    [Fact]
    public void BasicTest()
    {
        var cs = "InMemory";
        var sql = Sql.New(cs);
        var dbResult = sql.Select<DtoViewModel>("SELECT * FROM ....");
        Assert.NotNull(dbResult);
    }
}
