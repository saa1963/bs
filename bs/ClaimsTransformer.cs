using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace bs
{
    public class ClaimsTransformer : IClaimsTransformation
    {
        private IEnumerable<string> UsersNew, UsersPut, UsersWithdrawal;
        public ClaimsTransformer(string[] usersNew, string[] usersPut, string[] usersWithdrawal)
        {
            UsersNew = usersNew.Select(s => new string(s.ToUpper()));
            UsersPut = usersPut.Select(s => new string(s.ToUpper()));
            UsersWithdrawal = usersWithdrawal.Select(s => new string(s.ToUpper()));
        }
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var id = (ClaimsIdentity?)principal.Identity;
            
            var ci = new ClaimsIdentity(id.Claims, id.AuthenticationType, id.NameClaimType, id.RoleClaimType);
            
            if (UsersNew.Contains(ci.Name.ToUpper()))
            {
                ci.AddClaim(new Claim("permission", "MarkNew"));
            }
            if (UsersPut.Contains(ci.Name.ToUpper()))
            {
                ci.AddClaim(new Claim("permission", "MarkPut"));
            }
            if (UsersWithdrawal.Contains(ci.Name.ToUpper()))
            {
                ci.AddClaim(new Claim("permission", "MarkWithdrawal"));
            }
            var cp = new ClaimsPrincipal(ci);

            return Task.FromResult(cp);
        }
    }
}
