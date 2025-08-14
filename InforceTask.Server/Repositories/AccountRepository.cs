using Microsoft.AspNetCore.Identity;

namespace InforceTask.Server.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountRepository(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public Task<IdentityResult> CreateUserAsync(IdentityUser user, string password) =>
            _userManager.CreateAsync(user, password);
        public async Task DeleteUserAsync(IdentityUser user) =>
            await _userManager.DeleteAsync(user);

        public Task AddToRoleAsync(IdentityUser user, string role) =>
            _userManager.AddToRoleAsync(user, role);

        public Task<IdentityUser?> FindByEmailAsync(string email) =>
            _userManager.FindByEmailAsync(email);

        public Task<bool> IsInRoleAsync(IdentityUser user, string role) =>
            _userManager.IsInRoleAsync(user, role);

        public Task<SignInResult> SignInAsync(IdentityUser user, string password) =>
            _signInManager.PasswordSignInAsync(user, password, true, false);

        public Task SignOutAsync() =>
            _signInManager.SignOutAsync();
    }
}
