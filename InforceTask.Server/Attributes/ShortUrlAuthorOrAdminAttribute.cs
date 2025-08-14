using InforceTask.Server.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace InforceTask.Server.Attributes
{
    public class ShortUrlAuthorOrAdminAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var dbContext = context.HttpContext.RequestServices.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;
            var user = context.HttpContext.User;

            if (dbContext == null || user == null || !user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (user.IsInRole("Admin"))
                return;

            var email = user.Identity?.Name;
            if (string.IsNullOrEmpty(email))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var idRouteValue = context.RouteData.Values["id"]?.ToString();
            if (!int.TryParse(idRouteValue, out int shortUrlId))
            {
                context.Result = new BadRequestObjectResult("Invalid URL ID");
                return;
            }

            var shortUrl = dbContext.ShortUrls.FirstOrDefault(s => s.Id == shortUrlId);
            if (shortUrl == null)
            {
                context.Result = new NotFoundObjectResult("Short link not found");
                return;
            }

            if (!string.Equals(shortUrl.AuthorEmail, email, StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
