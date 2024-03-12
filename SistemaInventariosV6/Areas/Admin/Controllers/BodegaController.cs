using Microsoft.AspNetCore.Mvc;
using SistemaInventarios.AccesoDatos.Repositorio.IRepositorio;

namespace SistemaInventariosV6.Areas.Admin.Controllers
{
    //Se debe indicar esta etiqueta a cada controlador para indicar a que area pertenece.
    //Si no se indica se producirá un Error Not Found.
    [Area("Admin")]
    public class BodegaController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;

        public BodegaController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region API

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()//el "IActionResult" retorna una vista y objetos con formato json
        {
            var todos = await _unidadTrabajo.Bodega.ObtenerTodos();
            return Json(new { data = todos });//"data" es el nombre que se va usar en la javascript, por lo que si se cambia ese nombre aquí, se debe usar ese mismo en el javascript
        }

        #endregion
    }
}
