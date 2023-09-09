﻿using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;

namespace AWSDB.Models
{
    public class LeadDetailsEntity
    {
        [Key]
        public int id { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set;}  
    }
}
public class Articulo
{
	[Required]
	public string Nombre { get; set; }

	[Required]
	public string Precio { get; set; }
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