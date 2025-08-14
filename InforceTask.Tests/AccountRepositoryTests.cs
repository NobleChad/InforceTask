using InforceTask.Server.Repositories;
using InforceTask.Tests.Factories;
using Microsoft.AspNetCore.Identity;

namespace InforceTask.Tests
{
    public class AccountRepositoryTests
    {
        private readonly AccountRepository _repository;
        private readonly AccountRepositoryFactory _factory;

        public AccountRepositoryTests()
        {
            _factory = new AccountRepositoryFactory();
            _repository = new AccountRepository(_factory.UserManager, _factory.SignInManager);
        }

        [Fact]
        public async Task CreateUserAsync_AddsUserToDb()
        {
            var user = new IdentityUser { Email = "test@example.com" };
            var password = "Password123!";

            var result = await _repository.CreateUserAsync(user, password);

            Assert.True(result.Succeeded);
            Assert.NotNull(await _factory.UserManager.FindByEmailAsync(user.Email));
        }

        [Fact]
        public async Task DeleteUserAsync_RemovesUserFromDb()
        {
            var user = new IdentityUser { Email = "delete@example.com" };
            await _factory.UserManager.CreateAsync(user, "Password123!");

            await _repository.DeleteUserAsync(user);

            var found = await _factory.UserManager.FindByEmailAsync(user.Email);
            Assert.Null(found);
        }

        [Fact]
        public async Task AddToRoleAsync_AddsRoleToUser()
        {
            var user = new IdentityUser { Email = "role@example.com" };
            await _factory.UserManager.CreateAsync(user, "Password123!");
            await _factory.UserManager.AddToRoleAsync(user, "Admin");

            var inRole = await _repository.IsInRoleAsync(user, "Admin");
            Assert.True(inRole);
        }

        [Fact]
        public async Task FindByEmailAsync_ReturnsUser()
        {
            var user = new IdentityUser { Email = "find@example.com" };
            await _factory.UserManager.CreateAsync(user, "Password123!");

            var found = await _repository.FindByEmailAsync("find@example.com");
            Assert.Equal(user.Email, found.Email);
        }
    }
}
