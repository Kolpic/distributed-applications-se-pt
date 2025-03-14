using Common.Entities;
using Common.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProjectManagementAPI.ViewModels.RefreshTokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectManagementAPI.Controllers
{
    /// <summary>
    /// Handles refresh token operations for JWT authentication
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RefreshTokensController : ControllerBase
    {
        /// <summary>
        /// Issues a new JWT token using a refresh token
        /// </summary>
        /// <remarks>
        /// Use this endpoint to obtain a new JWT token when the current token has expired.
        /// No authentication is required for this endpoint.
        /// 
        /// Sample request:
        /// 
        ///     POST /api/RefreshTokens
        ///     {
        ///        "refreshToken": "9f8d7c6b-5a4e-3b2d-1c0f-9e8d7c6b5a4e"
        ///     }
        /// </remarks>
        /// <param name="model">The refresh token</param>
        /// <returns>A new JWT token and refresh token</returns>
        /// <response code="200">Returns a new JWT token and refresh token</response>
        /// <response code="400">If the refresh token is invalid</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] PostVM model)
      {
          ProjectManagementDbContext context = new ProjectManagementDbContext();
          RefreshToken t = context.RefreshTokens
              .Where(i =>
                      i.Token == model.RefreshToken)
              .FirstOrDefault();

          if (t == null)
            return BadRequest(ModelState);

          User u = context.Users
            .Where(u => u.Id == t.UserId)
            .FirstOrDefault();

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
              expires: DateTime.Now.AddMinutes(2),
              signingCredentials: signingCredentials
          );
          var jwtHandler = new JwtSecurityTokenHandler();
          var jwtToken = jwtHandler.WriteToken(token);

          foreach(var item in context.RefreshTokens.ToList())
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
