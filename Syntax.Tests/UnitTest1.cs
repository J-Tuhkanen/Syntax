using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Syntax.Core.Data;
using Syntax.Core.Models;
using Syntax.Core.Services.Base;
using System;

namespace Syntax.Tests
{
    public class Tests : TestBase
    {
        [SetUp]
        public async Task Setup()
        {
            var dbContext = GetService<ApplicationDbContext>();
            IUserService userService = GetService<IUserService>();

            if(await dbContext.Users.FirstOrDefaultAsync(_ => _.UserName == "TimoTest") == null)
            {
                var user = CreateUserInstance();

                var result = await userService.CreateUser(user, "TimoTest", "Testi123", "timo.testi@gmail.com");
            }

            await dbContext.Database.MigrateAsync();
        }

        [Test]
        public void Test1()
        {

            Assert.True(1 == 2);
        }

        private UserAccount CreateUserInstance()
        {
            try
            {
                return Activator.CreateInstance<UserAccount>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(UserAccount)}'. " +
                    $"Ensure that '{nameof(UserAccount)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }
    }
}