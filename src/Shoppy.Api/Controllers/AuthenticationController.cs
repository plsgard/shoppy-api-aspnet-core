using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shoppy.Api.Authentication;
using Shoppy.Api.Models.Authentication;
using Shoppy.Api.Models.Configuration;
using Shoppy.Application.Authentication.Dtos;
using Shoppy.Core.Users;

namespace Shoppy.Api.Controllers
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/auth")]
    public class AuthenticationController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IJwtFactory _jwtFactory;
        private readonly TokenAuthenticationOptions _jwtOptions;

        public AuthenticationController(SignInManager<User> signInManager, UserManager<User> userManager, IJwtFactory jwtFactory, IOptions<TokenAuthenticationOptions> jwtOptions)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
        }

        /// <summary>
        /// Generates an authentication token for the provided user login.
        /// </summary>
        /// <param name="model">Couple of login/password account infos.</param>
        /// <returns>A new authentication Bearer token.</returns>
        /// <response code="200">Returns the newly-generated token.</response>
        /// <response code="400">If the user account is invalid.</response>            
        [HttpPost("token")]
        [ProducesResponseType(typeof(TokenDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Token([FromBody]LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Username);
            if (user == null)
                return BadRequest();

            var result = await _signInManager.CheckPasswordSignInAsync
                (user, model.Password, true);

            if (!result.Succeeded)
                return Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);

            var generateClaimsIdentity = _jwtFactory.GenerateClaimsIdentity(model.Username, user.Id, roles);
            var jwt = await TokenDto.GenerateJwt(generateClaimsIdentity, _jwtFactory, model.Username, _jwtOptions);
            return Ok(jwt);
        }
    }
}
