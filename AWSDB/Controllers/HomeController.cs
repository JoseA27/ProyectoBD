using AWSDB.Data;
using AWSDB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting;

namespace AWSDB.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly ApplicationDBContext _db;
		private readonly IWebHostEnvironment _webHostEnvironment;
		string connetionString;
		public HomeController(ILogger<HomeController> logger, ApplicationDBContext db, IWebHostEnvironment webHostEnvironment)
		{
			_db = db;
			_logger = logger;
			_webHostEnvironment = webHostEnvironment;
			connetionString = "server=tecdb.ctfxom3mv69f.us-east-2.rds.amazonaws.com,1433;Database=WebLeads;TrustServerCertificate=True;User ID=admin;Password=tecaws123;";
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		public IActionResult Index()
		{
			//var getArticulo = _db.Articulo.FromSqlRaw("getArticulo").ToList();
			//return View(getArticulo);
			return View();
		}

		public IActionResult Volver()
		{
			return RedirectToAction("Index", "Home");
		}
		public IActionResult Create()
		{
			return View();
		}
		public IActionResult Upload()
		{
			return View();
		}
		public IActionResult CreateArticle()
		{
			return RedirectToAction("Create", "Home");
		}
		public IActionResult VistaUpload ()
		{
			return RedirectToAction("Upload", "Home");
		}

		public IActionResult actualizar(Articulo articulo)
		{
			if (validarDatos(articulo.Nombre, articulo.Precio) == false)
			{
				TempData["Message"] = "Ingrese la informacion del articulo de forma correcta. Nombre: Solo puede contener letras, espacio y guiones. Precio: Solo puede contener numeros enteros o decimales";
				return RedirectToAction("Create", "Home");
			}
			string nombre = articulo.Nombre;
			decimal precio = Convert.ToDecimal(articulo.Precio);


			using (SqlConnection connection = new SqlConnection(connetionString))
			{
				connection.Open();
				using (SqlCommand command = new SqlCommand("AddArticulo", connection))
				{
					command.CommandType = CommandType.StoredProcedure;

					command.Parameters.AddWithValue("@inNombre", nombre);
					command.Parameters.AddWithValue("@inPrecio", precio);
					command.Parameters.Add("@outResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

					command.ExecuteNonQuery();

					int resultCode = Convert.ToInt32(command.Parameters["@outResultCode"].Value);
					connection.Close();
					if (resultCode == 50002)
					{
						TempData["Message"] = "Nombre de articulo ya existe";
						return RedirectToAction("Create", "Home");
					}
					else
					{
						TempData["Message"] = "Insercion exitosa";
						return RedirectToAction("Index", "Home");
					}
				}
			}
		}
		public bool validarDatos(string nombre, string precio)
		{
			if (nombre == null || precio == null) { return false; }
			var regex = @"^(?!.*\s{2,})[a-zA-Z\-][a-zA-Z\- ]*[a-zA-Z\-]$";
			var match = Regex.Match(nombre, regex, RegexOptions.IgnoreCase);
			var regex2 = @"^(?:\d+|\d+\.\d+)$";
			var match2 = Regex.Match(precio, regex2, RegexOptions.IgnoreCase);
			if (match.Success && match2.Success)
			{
				return true;
			}
			return false;
		}


		public async Task<IActionResult> UploadFile(ArchivoViewModel model)
		{
			if (model.Archivo != null && model.Archivo.Length > 0)
			{
				// Leer el contenido del archivo XML
				using (var reader = new StreamReader(model.Archivo.OpenReadStream()))
				{
					string xmlContent = await reader.ReadToEndAsync();

					// Llamar al Stored Procedure con el contenido XML como parámetro
					using (SqlConnection connection = new SqlConnection(connetionString))
					{
						connection.Open();
						using (SqlCommand command = new SqlCommand("ProcesarXml", connection))
						{
							command.CommandType = CommandType.StoredProcedure;

							// Pasa el contenido XML como parámetro
							command.Parameters.AddWithValue("@Datos", xmlContent);

							// Configura el parámetro de salida para capturar la contraseña
							command.Parameters.Add("@outResult", SqlDbType.VarChar, 128).Direction = ParameterDirection.Output;

							command.ExecuteNonQuery();

							// Obtener la contraseña del parámetro de salida
							string password = Convert.ToString(command.Parameters["@outResult"].Value);

							// Realiza acciones adicionales si es necesario

							TempData["Message"] = "Carga de archivo exitosa y procesamiento XML completado. Contraseña de PRojas: " + password;
							return RedirectToAction("Index"); // Redirecciona a la página principal u otra página
						}
					}
				}
			}

			// Maneja el caso en que no se seleccionó ningún archivo
			TempData["Message"] = "Por favor, seleccione un archivo.";
			return RedirectToAction("VistaUpload");
		}

	}
}