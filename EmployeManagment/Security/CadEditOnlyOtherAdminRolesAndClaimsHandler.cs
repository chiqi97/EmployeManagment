using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeManagment.Security
{
    public class CanEditOnlyOtherAdminRolesAndClaimsHandler :
                AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public CanEditOnlyOtherAdminRolesAndClaimsHandler(
                IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                ManageAdminRolesAndClaimsRequirement requirement)
        {

            var loggedInAdminId = context.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value.ToString();

            var adminIdBeingEdited = httpContextAccessor.HttpContext
                .Request.Query["userId"].ToString();

            if (context.User.IsInRole("Admin")
                 && context.User.HasClaim(c => c.Type == "Edit Role" && c.Value == "true")
                 && adminIdBeingEdited.ToLower() != loggedInAdminId.ToLower())
            {
                context.Succeed(requirement);
            }
            // Jesli w pierwszym handlerze wyrzuci falsz to drugi sie nie wlaczy 
            // Jesli chcemy zeby sprawdzono drugi nie powinnismy dawac opcji wyrzucenia falszu
            // Jesli chceny zeby po falszu w pierwszym handlerze po context.Fail zostal
            // sprawdzany nalezy w startupie InvokeHandlerAfterFailure ustawic na false
            //else
            //{
            //    context.Fail();
            //}

            return Task.CompletedTask;
        }

    }
}