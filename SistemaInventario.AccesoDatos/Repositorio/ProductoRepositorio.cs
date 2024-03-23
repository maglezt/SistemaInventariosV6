using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaInventarios.AccesoDatos.Data;
using SistemaInventarios.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventarios.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventarios.AccesoDatos.Repositorio
{
    public class ProductoRepositorio : Repositorio<Producto>, IProductoRepositorio
    {
        private readonly ApplicationDbContext _db;

        public ProductoRepositorio(ApplicationDbContext db): base(db)
        {
            _db = db;
            
        }
        public void Actualizar(Producto producto)
        {
            var productoDB = _db.Productos.FirstOrDefault(b => b.Id == producto.Id); //Con esto se verifica si la bodega a actualizar existe en la base de datos, buscandolo por su ID
            if (productoDB != null) 
            {
                if(producto.ImagenUrl != null)
                {
                    productoDB.ImagenUrl = producto.ImagenUrl;
                }

                productoDB.NumeroSerie = producto.NumeroSerie;
                productoDB.Descripcion = producto.Descripcion;
                productoDB.Precio = producto.Precio;
                productoDB.Costo = producto.Costo;
                productoDB.CategoriaId = producto.CategoriaId;
                productoDB.MarcaId = producto.MarcaId;
                productoDB.PadreId = producto.PadreId;
                productoDB.Estado = producto.Estado;
                
                _db.SaveChanges();
            }
        }

        public IEnumerable<SelectListItem> ObtenerTodosDropDownLista(string obj)
        {
            switch(obj)
            {
                case "Categoria":
                    //En el Where se indica que solo se traiga las categorias activas.
                    //En el Select se está indicando que los elementos filtrados del modelo Categoria 
                    //al tipo SelectListItem donde en las propiedades Text y Value se indica el nombre
                    //y el ID del elemento respectivamente
                    return _db.Categorias.Where(c => c.Estado == true).Select(c => new SelectListItem
                    { 
                        Text = c.Nombre,
                        Value = c.Id.ToString()
                    });

                case "Marca":
                    return _db.Marcas.Where(m => m.Estado == true).Select(m => new SelectListItem
                    {
                        Text = m.Nombre,
                        Value = m.Id.ToString()
                    });

                case "Producto":
                    return _db.Productos.Where(m => m.Estado == true).Select(m => new SelectListItem
                    {
                        Text = m.Descripcion,
                        Value = m.Id.ToString()
                    });

                default:
                    return null;
            }
            
        }
    }
}
