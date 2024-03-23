using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventarios.Modelos
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="Numero de serie requerido")]
        [MaxLength(60)]
        public string NumeroSerie { get; set; }

        [Required(ErrorMessage = "Descripción es requerido")]
        [MaxLength(60)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Precio requerido")]
        public double Precio { get; set; }

        [Required(ErrorMessage = "Costo requerido")]
        public double Costo { get; set; }

        public string ImagenUrl { get; set; }

        [Required(ErrorMessage ="Estado Requerido")]
        public bool Estado { get; set; }

        [Required(ErrorMessage ="Categoria requerida")]
        public int CategoriaId { get; set; }

        [ForeignKey("CategoriaId")]//Esto relaciona la propiedad anterior con el modelo Categoria
        public Categoria Categoria { get; set; }

        [Required(ErrorMessage = "Marca requerida")]
        public int MarcaId { get; set; }

        [ForeignKey("MarcaId")]//Esto relaciona la propiedad anterior con el modelo Categoria
        public Marca Marca { get; set; }

        //Recursividad. Se pone el signo de interrogación despues del int para que esta propiedad permita nulo
        public int? PadreId { get; set; }

        public virtual Producto Padre { get; set; }
    }
}
