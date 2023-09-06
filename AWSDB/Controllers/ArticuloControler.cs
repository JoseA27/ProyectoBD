using AWSDB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AWSDB.Controllers
{
	public class ArticuloControler : Controller
	{

		private readonly ApplicationDBContext _db;

		public ArticuloControler (ApplicationDBContext db)
		{
			_db = db;
		}

		[HttpPost]
		public async Task<IActionResult> Create(Articulo article)
		{
			if (ModelState.IsValid)
			{
				await _db.AddAsync(article);
				await _db.SaveChangesAsync();

				return RedirectToAction("Index", "Home");
			}

			return View(article);
		}
		public IActionResult Index()
		{
			var getArticulo = _db.Articulo.FromSqlRaw("getArticulo").ToList();
			return View(getArticulo);
		}
	}

}
