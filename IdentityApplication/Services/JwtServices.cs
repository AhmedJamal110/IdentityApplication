using IdentityApplication.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdentityApplication.API.Services
{
    public class JwtServices
    {
        private readonly IConfiguration _conig;
        private readonly UserManager<AppUser> _userManager;
        private readonly SymmetricSecurityKey _key;

        public  JwtServices(IConfiguration conig , UserManager<AppUser> userManager)
        {
            _conig = conig;
            _userManager = userManager;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_conig["Token:key"]));
        }
        public async Task<string> CreateJwt(AppUser user)
        {

            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier , user.Id),
                new Claim(ClaimTypes.Email , user.UserName),
                new Claim(ClaimTypes.GivenName , user.FirstName),
                new Claim(ClaimTypes.Surname , user.LastName),
            };

            var roles = await _userManager.GetRolesAsync(user);
            userClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));


            var Token = new JwtSecurityToken(
                claims: userClaims,
                issuer: _conig["Token:ValidIssuer"],
                audience: _conig["Token:ValidAudiance"],
                signingCredentials: new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature),
                expires: DateTime.Now.AddDays(double.Parse(_conig["Token:ExpirationTime"]))
                );

            return new JwtSecurityTokenHandler().WriteToken(Token);







        }


    }
}
