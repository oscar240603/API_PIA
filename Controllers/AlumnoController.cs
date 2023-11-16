using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using WEB_API_ALUMNOS.Models;

namespace WEB_API_ALUMNOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlumnoController : ControllerBase
    {
        private readonly string cadenaSQL;
        public AlumnoController(IConfiguration config)
        {
            cadenaSQL = config.GetConnectionString("CadenaSQL");
        }

        [HttpGet]
        [Route("Lista")]
        public IActionResult Lista()
        {
            List<Alumno> lista = new List<Alumno>();

            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_lista_alumnos", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using(var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(new Alumno()
                            {
                                IdMatricula = Convert.ToInt32(rd["IdMatricula"]),
                                Nombre = rd["Nombre"].ToString(),
                                Apaterno = rd["Apaterno"].ToString(),
                                Amaterno = rd["Amaterno"].ToString(),
                                Correo_Uni = rd["Correo_Uni"].ToString(),
                                FechaRegistro = rd["FechaRegistro"].ToString(),


                            });
                        }
                    }
                }
                return StatusCode(StatusCodes.Status200OK, new {mensaje = "ok", Response = lista});



            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message, Response = lista });
            }


        }

        [HttpGet]
        [Route("Obtener/{IdMatricula:int}")]
        public IActionResult Obtener(int IdMatricula)
        {
            List<Alumno> lista = new List<Alumno>();
            Alumno alumno = new Alumno();

            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_lista_alumnos", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(new Alumno()
                            {
                                IdMatricula = Convert.ToInt32(rd["IdMatricula"]),
                                Nombre = rd["Nombre"].ToString(),
                                Apaterno = rd["Apaterno"].ToString(),
                                Amaterno = rd["Amaterno"].ToString(),
                                Correo_Uni = rd["Correo_Uni"].ToString(),
                                FechaRegistro = rd["FechaRegistro"].ToString(),


                            });
                        }
                    }
                }
                alumno = lista.Where(item => item.IdMatricula == IdMatricula).FirstOrDefault();

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", Response = alumno });



            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message, Response = alumno });
            }


        }

        [HttpPost]
        [Route("Guardar")]
        public IActionResult Guardar([FromBody] GuardarAlumno objeto)
        {
 

            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("spAddAlumno", conexion);
                    cmd.Parameters.AddWithValue("Nombre", objeto.Nombre);
                    cmd.Parameters.AddWithValue("Apaterno", objeto.Apaterno);
                    cmd.Parameters.AddWithValue("Amaterno", objeto.Amaterno);
                    cmd.Parameters.AddWithValue("Correo_Uni", objeto.Correo_Uni);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.ExecuteNonQuery();
                }
                
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok" });

            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message});
            }


        }

        [HttpPut]
        [Route("Editar")]
        public IActionResult Editar([FromBody] GuardarAlumno objeto)
        {


            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("spModificar", conexion);
                    cmd.Parameters.AddWithValue("IdMatricula", objeto.IdMatricula == 0 ? DBNull.Value : objeto.IdMatricula);
                    cmd.Parameters.AddWithValue("Nombre", objeto.Nombre is null ? DBNull.Value : objeto.Nombre);
                    cmd.Parameters.AddWithValue("Apaterno", objeto.Apaterno is null ? DBNull.Value : objeto.Apaterno);
                    cmd.Parameters.AddWithValue("Amaterno", objeto.Amaterno is null ? DBNull.Value : objeto.Amaterno);
                    cmd.Parameters.AddWithValue("Correo_Uni", objeto.Correo_Uni is null ? DBNull.Value : objeto.Correo_Uni);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.ExecuteNonQuery();
                }

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Alumno modificado" });

            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }


        }

        [HttpDelete]
        [Route("Eliminar/{IdMatricula:int}")]
        public IActionResult Eliminar(int IdMatricula)
        {


            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("spEliminar", conexion);
                    cmd.Parameters.AddWithValue("IdMatricula", IdMatricula);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.ExecuteNonQuery();
                }

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Alumno eliminado" });

            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }


        }




    }
}
