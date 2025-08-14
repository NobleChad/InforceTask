namespace InforceTask.Server.Repositories
{
    public interface IUrlValidationService
    {
        Task<(bool IsValid, string ErrorMessage)> ValidateUrlAsync(string url);
    }
}
