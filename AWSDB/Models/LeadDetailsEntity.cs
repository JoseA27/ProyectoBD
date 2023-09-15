using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using AWSDB.Models;

namespace AWSDB.Models
{
	public class LeadDetailsEntity
	{
		[Key]
		public int id { get; set; }
		public string Codigo { get; set; }
		public string Nombre { get; set; }
		public string ClaseArticulo { get; set; }
		public decimal Precio { get; set; }
	}

    public class ClaseArticulo
    {
        [Key]
        public int id { get; set; }
        public string Nombre { get; set; }
    }
}

public class CombinedViewModel
{
	public IEnumerable<LeadDetailsEntity> LeadDetails { get; set; }

	public Articulo NewArticulo { get; set; }

    public List<AWSDB.Models.ClaseArticulo> NewCA { get; set; }


	public string Codigo { get; set;}

	public string UserName { get; set;}
}

public class Articulo
{
	[Required]
	public int id { get; set; }

	[Required]
	public string Codigo { get; set; }

	[Required]
	public string Nombre { get; set; }
	[Required]
	public string ClaseArticulo { get; set; }

	[Required]
	public string Precio { get; set; }

	[Required]
	public string Cantidad { get; set; }
}
public class ArchivoViewModel
{
	public IFormFile Archivo { get; set; }
}

public class Usuario
{
	[Required]
	public string Nombre { get; set; }

	[Required]
	public string Password { get; set; }
}