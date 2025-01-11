using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Business.Utils
{
    public static class ClaimUtil
    {
        public static string GetClaim(this ClaimsPrincipal claimsPrincipal, string jwtClaim)
        {
            var allClaims = claimsPrincipal.Claims.ToList();
            var claim = claimsPrincipal.Claims.Where(c => c.Type == jwtClaim).FirstOrDefault();

            if (claim == null)
            {
                throw new Exception("Claim not found - " + jwtClaim.ToString());
            }

            return claim.Value;
        }
    }
}
