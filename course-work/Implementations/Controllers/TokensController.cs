using Common.Entities;
using Common.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using ProjectManagementAPI.ViewModels.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectManagementAPI.Controllers
{
    /// <summary>
    /// Handles authentication and issuance of JWT tokens
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class TokensController : ControllerBase
    {
        /// <summary>
        /// Authenticates a user and issues a JWT token
        /// </summary>
        /// <remarks>
        /// Use this endpoint to authenticate a user and obtain a JWT token for API access.
        /// No authentication is required for this endpoint.
        /// 
        /// Sample request:
        /// 
        ///     POST /api/Tokens
        ///     {
        ///        "username": "johndoe",
        ///        "password": "securepassword"
        ///     }
        /// </remarks>
        /// <param name="model">The user credentials</param>
        /// <returns>A JWT token and refresh token</returns>
        /// <response code="200">Returns the JWT token and refresh token</response>
        /// <response code="400">If the credentials are invalid</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody]PostVM model)
        {
            ProjectManagementDbContext context = new ProjectManagementDbContext();
            User u = context.Users
                .Where(i =>
                        i.Username == model.Username &&
                        i.Password == model.Password)
                .FirstOrDefault();

            if (u == null)
                return BadRequest(ModelState);

            var claims = new Claim[]
            {
                new Claim("LoggedUserId", u.Id.ToString())
            };

            var symmetricKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("IwaslikewhateverandeveryonewhatevereD"));
            var signingCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                "fmi",
                "project-management-app",
                claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: signingCredentials
            );
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.WriteToken(token);

            foreach (var item in context.RefreshTokens.ToList())
              item.Status = RefreshToken.TokenStatus.Used;

            RefreshToken refreshToken = new RefreshToken();
            refreshToken.UserId = u.Id;
            refreshToken.Token = Guid.NewGuid().ToString();
            refreshToken.Status = RefreshToken.TokenStatus.Pending;

            context.RefreshTokens.Add(refreshToken);
            context.SaveChanges();

            return Ok(
                new
                {
                  success = true,
                  token = jwtToken,
                  refreshToken = refreshToken
                });
          }
    }
}
