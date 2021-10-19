using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Nebula.Data;
using Nebula.Data.ViewModels;

namespace Nebula.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public AuthController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] AuthLogin model)
        {
            var user = _context.Users.SingleOrDefault(m =>
                m.UserName.Equals(model.UserName)
                && m.Suspended.Equals(false) && m.SoftDeleted.Equals(false));

            // verificar usuario y contraseña.
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                return NotFound(new { Ok = false, Msg = "Usuario/Contraseña Invalida!" });
            }

            // Leemos el secretKey desde nuestro appsettings.json
            var secretKey = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("SecretKey"));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserName)
                }),
                // Nuestro token va a durar un día
                Expires = DateTime.UtcNow.AddHours(12),
                // Credenciales para generar el token usando nuestro secretKey y el algoritmo hash 256
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var createdToken = tokenHandler.CreateToken(tokenDescriptor);

            // Token generated.
            return Ok(new
            {
                token = tokenHandler.WriteToken(createdToken)
            });
        }
    }
}