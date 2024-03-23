using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventarios.Modelos.ViewModels
{
    public class ProductoVM
    {
        public Producto Producto { get; set; }

        //Estas propiedades son para obtener el listado de los elementos del modelo Categoria y Marca respectivamente
        public IEnumerable<SelectListItem> CategoriaLista { get; set; }
        public IEnumerable<SelectListItem> MarcaLista { get; set; }
        public IEnumerable<SelectListItem> PadreLista { get; set; }

    }
}
