using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ToDoApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly string _secretKey = "supersecretkeythatisatleast128bitslong"; // مفتاح سري لتوقيع JWT

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
                return BadRequest("Invalid credentials.");

            // تحقق من المستخدم وصلاحياته (مثال بسيط، يمكن ربطه بقاعدة بيانات أو مصدر خارجي)
            var userRole = model.Username == "admin" ? "Owner" : "Guest"; // صلاحيات الافتراضية

            // إنشاء الـ JWT بناءً على الدور
            var token = GenerateJwtToken(model.Username, userRole);

            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(string username, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role), // إضافة دور المستخدم
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "https://localhost:7112", // يمكنك تغيير هذا حسب الحاجة
                audience: "https://localhost:7112/api", // يجب أن يتوافق مع API الذي تستخدمه
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

}
