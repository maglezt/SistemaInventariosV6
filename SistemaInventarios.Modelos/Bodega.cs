using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventarios.Modelos
{
    public class Bodega
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="El nombre es requerido")]
        [MaxLength(60, ErrorMessage ="El nombre debe ser máximo 60 caracteres")]
        public string Nombre { get; set; }

        [MaxLength(100, ErrorMessage = "La descripción debe ser máximo 100 caracteres")]
        public string Descripcion { get; set; }
        public bool Estado { get; set; }
    }
}
