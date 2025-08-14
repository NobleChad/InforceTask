using InforceTask.Server.Constants;
using InforceTask.Server.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace InforceTask.Tests.Factories
{
    public class AccountRepositoryFactory
    {
        public ApplicationDbContext Context { get; }
        public UserManager<IdentityUser> UserManager { get; }
        public RoleManager<IdentityRole> RoleManager { get; }
        public SignInManager<IdentityUser> SignInManager { get; }

        public AccountRepositoryFactory()
        {
            // In-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            Context = new ApplicationDbContext(options);

            // UserManager
            var userStore = new UserStore<IdentityUser>(Context);
            UserManager = new UserManager<IdentityUser>(
                userStore,
                null,
                new PasswordHasher<IdentityUser>(),
                Array.Empty<IUserValidator<IdentityUser>>(),
                Array.Empty<IPasswordValidator<IdentityUser>>(),
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                null,
                null
            );

            // RoleManager
            var roleStore = new RoleStore<IdentityRole>(Context);
            RoleManager = new RoleManager<IdentityRole>(
                roleStore,
                Array.Empty<IRoleValidator<IdentityRole>>(),
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                null
            );

            // Seed roles
            foreach (var roleName in Enum.GetNames(typeof(Roles)))
            {
                RoleManager.CreateAsync(new IdentityRole(roleName)).Wait();
            }
        }
    }
}
