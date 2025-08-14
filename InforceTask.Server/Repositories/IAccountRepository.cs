using Microsoft.AspNetCore.Identity;

namespace InforceTask.Server.Repositories
{
    public interface IAccountRepository
    {
        Task<IdentityResult> CreateUserAsync(IdentityUser user, string password);
        Task DeleteUserAsync(IdentityUser user);
        Task AddToRoleAsync(IdentityUser user, string role);
        Task<IdentityUser?> FindByEmailAsync(string email);
        Task<bool> IsInRoleAsync(IdentityUser user, string role);
        Task<SignInResult> SignInAsync(IdentityUser user, string password);
        Task SignOutAsync();
    }
}
