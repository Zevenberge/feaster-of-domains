using FeasterOfDomains.Users.Infrastructure;
using FeasterOfDomains.Users.Web.Models;
using IdentityModel;
/* using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Test;*/
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FeasterOfDomains.Users.Web.Controllers
{
    /// <summary>
    /// This sample controller implements a typical login/logout/provision workflow for local and external accounts.
    /// The login service encapsulates the interactions with the user data store. This data store is in-memory only and cannot be used for production!
    /// The interaction service provides a way for the UI to communicate with identityserver for validation and context retrieval
    /// </summary>
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<FeasterUser> _users;
        //private readonly IIdentityServerInteractionService _interaction;
        //private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AccountController> _logger;

        //private readonly IEventService _events;

        public AccountController(
            //IIdentityServerInteractionService interaction,
            //IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IHttpContextAccessor httpContextAccessor,
            //IEventService events,
            UserManager<FeasterUser> users,
            ILogger<AccountController> logger)
        {
            logger.LogInformation("Creating account controller");
            //_interaction = interaction;
            //_clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _httpContextAccessor = httpContextAccessor;
            //_events = events;
            _users = users;
            _logger = logger;
        }

        /// <summary>
        /// Entry point into the login workflow
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            _logger.LogInformation("Retrieving log in screen");
            // build a model so we know what to show on the login page
            var vm = await BuildLoginViewModelAsync(returnUrl);
            return View(vm);
        }

        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model, CancellationToken token = default(CancellationToken))
        {
            _logger.LogInformation("Trying to log in for user " + model.Username);
            // check if we are in the context of an authorization request
            //var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            if (ModelState.IsValid)
            {
                _logger.LogDebug("The model is valid");
                // validate username/password against in-memory store
                var user = await _users.FindByNameAsync(model.Username);
                if (await _users.CheckPasswordAsync(user, model.Password))
                {
                    _logger.LogDebug("The password is valid");
                    //await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName));

                    // only set explicit expiration here if user chooses "remember me". 
                    // otherwise we rely upon expiration configured in cookie middleware.
                    AuthenticationProperties props = null;

                    // issue authentication cookie with subject ID and username
                    await _httpContextAccessor.HttpContext.SignInAsync(user.Id, user.UserName, props);

                    

                    // request for a local page
                    if (Url.IsLocalUrl(model.ReturnUrl))
                    {
                        _logger.LogInformation("Log in succeeded, going to " + model.ReturnUrl);
                        return Redirect(model.ReturnUrl);
                    }
                    else if (string.IsNullOrEmpty(model.ReturnUrl))
                    {
                        _logger.LogInformation("Log in succeeded. Going home.");
                        return Redirect("~/");
                    }
                    else
                    {
                        // user might have clicked on a malicious link - should be logged
                        throw new Exception("invalid return URL");
                    }
                }
                _logger.LogDebug("Log in failed");
                //await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials"));
                ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);
                if(ModelState.IsValid) throw new Exception("MS PLS");
            }
            _logger.LogInformation("Log in failed. Trying again.");
            // something went wrong, show form with error
            var vm = await BuildLoginViewModelAsync(model);
            vm.Username = AccountOptions.InvalidCredentialsErrorMessage;
            return View(vm);
        }

        
        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            // build a model so the logout page knows what to display
            var vm = await BuildLogoutViewModelAsync(logoutId);

            if (vm.ShowLogoutPrompt == false)
            {
                // if the request for logout was properly authenticated from IdentityServer, then
                // we don't need to show the prompt and can just log the user out directly.
                return await Logout(vm);
            }

            return View(vm);
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            // build a model so the logged out page knows what to display
            var vm = await BuildLoggedOutViewModelAsync(model.LogoutId);

            if (User?.Identity.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await _httpContextAccessor.HttpContext.SignOutAsync();

                // raise the logout event
                //await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
            }

            // check if we need to trigger sign-out at an upstream identity provider
            if (vm.TriggerExternalSignout)
            {
                // build a return URL so the upstream provider will redirect back
                // to us after the user has logged out. this allows us to then
                // complete our single sign-out processing.
                string url = Url.Action("Logout", new { logoutId = vm.LogoutId });

                // this triggers a redirect to the external provider for sign-out
                return SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
            }

            return View("LoggedOut", vm);
        }



        /*****************************************/
        /* helper APIs for the AccountController */
        /*****************************************/
        private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
        {
            //var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            /* if (context?.IdP != null)
            {
                //var local = context.IdP == IdentityServerConstants.LocalIdentityProvider;

                // this is meant to short circuit the UI and only trigger the one external IdP
                var vm = new LoginViewModel
                {
                    ReturnUrl = returnUrl,
                    Username = context?.LoginHint,
                };

                return vm;
            }*/

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            return new LoginViewModel
            {
                ReturnUrl = returnUrl,
                //Username = context?.LoginHint
            };
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            var vm = await BuildLoginViewModelAsync(model.ReturnUrl);
            vm.Username = model.Username;
            return vm;
        }

        private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

            if (User?.Identity.IsAuthenticated != true)
            {
                // if the user is not authenticated, then just show logged out page
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            /* var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                vm.ShowLogoutPrompt = false;
                return vm;
            }*/

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return vm;
        }

        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            //var logout = await _interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
            //    PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
            //    ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
            //    SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId
            };

            if (User?.Identity.IsAuthenticated == true)
            {
                /*var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServer4.IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignout)
                    {
                        if (vm.LogoutId == null)
                        {
                            // if there's no current logout context, we need to create one
                            // this captures necessary info from the current logged in user
                            // before we signout and redirect away to the external IdP for signout
                            vm.LogoutId = await _interaction.CreateLogoutContextAsync();
                        }

                        vm.ExternalAuthenticationScheme = idp;
                    }
                }*/
            }

            return vm;
        }
}
}