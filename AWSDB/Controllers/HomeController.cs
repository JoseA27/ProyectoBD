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


		[HttpPost]
		public async Task<IActionResult> UploadFile(ArchivoViewModel model)
		{
			if (model.Archivo != null && model.Archivo.Length > 0)
			{
				// Obtén el nombre del archivo y la ruta donde se guardará en el servidor
				string fileName = Path.GetFileName(model.Archivo.FileName);
				string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);

				// Guarda el archivo en el servidor
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await model.Archivo.CopyToAsync(stream);
				}

				// Realiza acciones adicionales si es necesario

				TempData["Message"] = "Carga de archivo exitosa";
				return RedirectToAction("Index"); // Redirecciona a la página principal u otra página
			}

			// Maneja el caso en que no se seleccionó ningún archivo
			TempData["Message"] = "Por favor, seleccione un archivo.";
			return RedirectToAction("VistaUpload");
		}

	}
}