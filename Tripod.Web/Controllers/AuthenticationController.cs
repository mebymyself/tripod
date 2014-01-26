﻿using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Tripod.Domain.Security;

namespace Tripod.Web.Controllers
{
    public partial class AuthenticationController : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;
        private readonly IProcessValidation _validation;

        [UsedImplicitly]
        public AuthenticationController(IProcessQueries queries, IProcessCommands commands, IProcessValidation validation)
        {
            _queries = queries;
            _commands = commands;
            _validation = validation;
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost, Route("account/link-login")]
        public virtual ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action(MVC.Authentication.LinkLoginCallback()), User.Identity.GetUserId());
        }

        [Authorize]
        [HttpGet, Route("account/link-login/complete")]
        public virtual async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await _queries.Execute(new PrincipalRemoteMembershipTicket(User));
            if (loginInfo == null) return RedirectToAction(await MVC.Account.Manage(AccountController.ManageMessageId.Error));

            var command = new CreateRemoteMembership
            {
                Principal = User,
            };
            var validation = _validation.Validate(command);
            if (!validation.IsValid) return RedirectToAction(await MVC.Account.Manage(AccountController.ManageMessageId.Error));

            await _commands.Execute(command);
            return RedirectToAction(await MVC.Account.Manage());
        }

        [HttpGet, Route("account/external-login/failed")]
        public virtual ActionResult ExternalLoginFailure()
        {
            return View(MVC.Account.Views.ExternalLoginFailure);
        }
    }
}