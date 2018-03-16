using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shoppy.Api.Authentication;
using Shoppy.Api.Models.Configuration;

namespace Shoppy.Api.Models.Authentication
{
    [JsonObject]
    public class TokenDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("auth_token")]
        public string Token { get; set; }
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        public static async Task<TokenDto> GenerateJwt(ClaimsIdentity identity, IJwtFactory jwtFactory, string userName, TokenAuthenticationOptions jwtOptions)
        {
            return new TokenDto
            {
                Id = identity.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value,
                Token = await jwtFactory.GenerateEncodedToken(userName, identity),
                ExpiresIn = (int)jwtOptions.ValidFor.TotalSeconds
            };
        }
    }
}
