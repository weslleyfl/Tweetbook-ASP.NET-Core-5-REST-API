using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Tweetbook.Web.Pages
{
    /// <summary>
    /// https://code-maze.com/external-identity-provider-aspnet-core-identity/
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;

        public string ReturnUrl { get; set; } = "/";

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public IndexModel(ILogger<IndexModel> logger, SignInManager<IdentityUser> signInManager)
        {
            _logger = logger;
            _signInManager = signInManager;
        }

        public async Task OnGetAsync(string returnUrl = null)
        {

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public void OnPost()
        {

        }
    }
}
