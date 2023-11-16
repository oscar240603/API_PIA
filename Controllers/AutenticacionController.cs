using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WEB_API_ALUMNOS.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace WEB_API_ALUMNOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacionController : ControllerBase
    {
        private readonly string cadenaSQL;
        private readonly string secrectkey;
        
        public AutenticacionController(IConfiguration config)
        {
            secrectkey = config.GetSection("settings").GetSection("secretkey").ToString();
            cadenaSQL = config.GetConnectionString("CadenaSQL");

        }

        [HttpPost]
        [Route("Validar")]
        public IActionResult Validar([FromBody] Cuenta request)
        {
            
            var conexion = new SqlConnection(cadenaSQL);

            
            using (SqlConnection connection = new SqlConnection(cadenaSQL))
            {
                connection.Open();

                // Consulta para verificar las credenciales
                string query = "SELECT COUNT(*) FROM Credenciales WHERE Usuario = @Usuario AND Contra = @Contra";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Usuario", request.Usuario);
                    command.Parameters.AddWithValue("@Contra", request.Contra);

                    int count = (int)command.ExecuteScalar();

                    if (count > 0)
                    {
                        // Credenciales válidas
                        var keyBytes = Encoding.ASCII.GetBytes(secrectkey);
                        var claims = new ClaimsIdentity();
                        claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, request.Usuario));

                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = claims,
                            Expires = DateTime.UtcNow.AddMinutes(5),
                            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
                        };

                        var tokenHandler = new JwtSecurityTokenHandler();
                        var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

                        string tokenCreado = tokenHandler.WriteToken(tokenConfig);

                        return StatusCode(StatusCodes.Status200OK, new { token = tokenCreado });
                    }
                }
            }

            // Credenciales inválidas
            return StatusCode(StatusCodes.Status401Unauthorized, new { token = "" });
        }



    }
}
