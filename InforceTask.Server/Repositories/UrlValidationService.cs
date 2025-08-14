namespace InforceTask.Server.Repositories
{
    public class UrlValidationService : IUrlValidationService
    {
        private readonly string[] _allowedTlds = { ".com", ".net", ".org", ".io", ".gov", ".edu", ".co", ".us", ".uk", ".tv" };

        public Task<(bool IsValid, string ErrorMessage)> ValidateUrlAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return Task.FromResult((false, "URL cannot be empty"));

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult) ||
                !(uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            {
                return Task.FromResult((false, "Invalid URL format. Only HTTP/HTTPS URLs are allowed."));
            }

            if (string.IsNullOrWhiteSpace(uriResult.Host) || !uriResult.Host.Contains('.') || uriResult.Host.Contains(" "))
                return Task.FromResult((false, "URL host is not valid."));

            if (!_allowedTlds.Any(tld => uriResult.Host.EndsWith(tld, StringComparison.OrdinalIgnoreCase)))
                return Task.FromResult((false, "URL must have a valid top-level domain (e.g., .com, .net, .org)."));

            return Task.FromResult((true, string.Empty));
        }
    }
}
