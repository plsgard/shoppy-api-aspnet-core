using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Shoppy.Api.Configurations
{
    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (context.ApiDescription.ControllerAttributes().OfType<AuthorizeAttribute>().Any() && !context.ApiDescription.ActionAttributes().OfType<AllowAnonymousAttribute>().Any() || context.ApiDescription.ActionAttributes().OfType<AuthorizeAttribute>().Any())
            {
                var roles = context.ApiDescription.ControllerAttributes().OfType<AuthorizeAttribute>()
                    .Select(c => c.Roles).Union(context.ApiDescription.ActionAttributes().OfType<AuthorizeAttribute>()
                        .Select(c => c.Roles)).Distinct().Where(c => !string.IsNullOrWhiteSpace(c));

                var policies = context.ApiDescription.ControllerAttributes().OfType<AuthorizeAttribute>()
                    .Select(c => c.Policy).Union(context.ApiDescription.ActionAttributes().OfType<AuthorizeAttribute>()
                        .Select(c => c.Policy)).Distinct().Where(c => !string.IsNullOrWhiteSpace(c));

                var allRights = roles.Union(policies);

                if (!operation.Responses.ContainsKey(((int)HttpStatusCode.Unauthorized).ToString()))
                    operation.Responses.Add(((int)HttpStatusCode.Unauthorized).ToString(), new Response { Description = HttpStatusCode.Unauthorized.ToString() });
                if (!operation.Responses.ContainsKey(((int)HttpStatusCode.Forbidden).ToString()))
                    operation.Responses.Add(((int)HttpStatusCode.Forbidden).ToString(), new Response { Description = HttpStatusCode.Forbidden.ToString() });
                operation.Security = new List<IDictionary<string, IEnumerable<string>>>
                {
                    new Dictionary<string, IEnumerable<string>>
                    {
                        {"Bearer", allRights}
                    }
                };
            }
            // Policy names map to scopes
            //var controllerScopes = context.ApiDescription.ControllerAttributes()
            //    .OfType<AuthorizeAttribute>()
            //    .Select(attr => attr.Policy);

            //var actionScopes = context.ApiDescription.ActionAttributes()
            //    .OfType<AuthorizeAttribute>()
            //    .Select(attr => attr.Policy);

            //var requiredScopes = controllerScopes.Union(actionScopes).Distinct();

            //if (requiredScopes.Any())
            //{
            //    operation.Responses.Add("401", new Response { Description = "Unauthorized" });
            //    operation.Responses.Add("403", new Response { Description = "Forbidden" });

            //    operation.Security = new List<IDictionary<string, IEnumerable<string>>>();
            //    operation.Security.Add(new Dictionary<string, IEnumerable<string>>
            //    {
            //        { "oauth2", requiredScopes }
            //    });
            //}
        }
    }
}