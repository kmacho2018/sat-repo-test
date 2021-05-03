using Sat.Recruitment.Api.Controllers;

using Xunit;

namespace Sat.Recruitment.Test
{
    [CollectionDefinition("Tests", DisableParallelization = true)]
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var userController = new UserController();

            var result = userController.CreateUser(new Data.Models.User() { Name = "Mike", Email = "mike@gmail.com", Address = "Av. Juan G", Phone = "+349 1122354215", UserType = "Normal", Money = 124 }).Result;

            Assert.Equal(true, result.Value.IsSuccess);
            Assert.Equal("User Created", result.Value.Errors);
        }

        [Fact]
        public void Test2()
        {
            var userController = new UserController();

            var result = userController.CreateUser(new Data.Models.User() { Name = "Agustina", Email = "Agustina@gmail.com", Address = "Av. Juan G", Phone = "+349 1122354215", UserType = "Normal", Money = 124 }).Result;

            Assert.Equal(false, result.Value.IsSuccess);
            Assert.Equal("The user is duplicated", result.Value.Errors);
        }
    }
}