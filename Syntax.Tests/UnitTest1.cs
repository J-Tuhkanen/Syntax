using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Syntax.Core.Data;

namespace Syntax.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=Syntax-Test;Trusted_Connection=True;MultipleActiveResultSets=true");
            optionsBuilder.UseLazyLoadingProxies();


            var dbContext = new ApplicationDbContext(optionsBuilder.Options);
            dbContext.Database.Migrate();
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}