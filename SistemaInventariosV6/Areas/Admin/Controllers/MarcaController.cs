using Microsoft.AspNetCore.Mvc;
using SistemaInventarios.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventarios.Modelos;
using SistemaInventarios.Utilidades;

namespace SistemaInventariosV6.Areas.Admin.Controllers
{
    //Se debe indicar esta etiqueta a cada controlador para indicar a que area pertenece.
    //Si no se indica se producirá un Error Not Found.
    [Area("Admin")]
    public class MarcaController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;

        public MarcaController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }

        public IActionResult Index()
        {
            return View();
        }

        //Este metodo es de tipo GET
        public async Task<IActionResult> Upsert(int? id)
        {
            Marca marca = new Marca();

            if(!id.HasValue)
            {
                //Crear una nueva bodega
                marca.Estado = true;
                return View(marca);
            }
            
            //Se actualiza bodega
            marca = await _unidadTrabajo.Marca.Obtener(id.GetValueOrDefault());
            if (marca == null)
                return NotFound();

            return View(marca);
        }

        [HttpPost] //hay que indicar esta etiqueta al metodo para indicar que es de tipo POST
        [ValidateAntiForgeryToken]//esto evita las falsificaciones de solicitudes de otro sitio que está intentando cargar datos en nuestra página
        public async Task<IActionResult> Upsert(Marca marca)
        {
            if(ModelState.IsValid) //valida que el modelo este correcto en cada una de sus propiedades
            { 
                if(marca.Id == 0)
                {
                    await _unidadTrabajo.Marca.Agregar(marca);
                    TempData[DS.Exitosa] = "Marca creada exitosamente";
                }
                else
                {
                    _unidadTrabajo.Marca.Actualizar(marca);
                    TempData[DS.Exitosa] = "Categoría actualizada exitosamente";
                }

                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));//redirecciona a Index
            }

            TempData[DS.Error] = "Error al grabar la marca";
            return View(marca);
        }

        #region API

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()//el "IActionResult" retorna una vista y objetos con formato json
        {
            var todos = await _unidadTrabajo.Marca.ObtenerTodos();
            return Json(new { data = todos });//"data" es el nombre que se va usar en la javascript, por lo que si se cambia ese nombre aquí, se debe usar ese mismo en el javascript
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            //Se obtiene la categoria según el ID indicado
            var marcaDB = await _unidadTrabajo.Marca.Obtener(id);

            //Si no existe 
            if(marcaDB == null)
            {
                //Se indica por medio de javascript un mensaje indicando el error
                //y se finaliza el proceso
                return Json(new { success = false, message = "Error al borrar la marca" });
            }

            //Si si existe la bodega, se realiza la eliminación
            _unidadTrabajo.Marca.Remover(marcaDB);

            //Se guardan los cambios.
            await _unidadTrabajo.Guardar();

            //Se indica un mensaje correspondiente a la eliminación
            return Json(new { success = true, message = "Marca \"" + marcaDB.Nombre + "\" borrada exitosamente" });
        }

        [ActionName("ValidarNombre")]//se indica este Tag para poder referenciarlo en el javascript desde la vista upsert
        public async Task<IActionResult> ValidarNombre(string nombre, int id=0)
        {
            bool existeMarca = false;
            var lista = await _unidadTrabajo.Marca.ObtenerTodos();

            //Si es una categoria  nueva
            if (id == 0)
            {
                //Se indica si existe la categoria que se quiere registrar
                existeMarca = lista.Any(b=>b.Nombre.ToLower().Trim().Equals(nombre.ToLower().Trim()));
            }
            //pero si es una bodega existente
            else
            {
                //Se indica si existe la categoria que se quiere modificar
                existeMarca = lista.Any(b => b.Nombre.ToLower().Trim().Equals(nombre.ToLower().Trim()) && b.Id != id);
            }

            //Se retorna un json indicando si existe o no la categoria  indicada
            return Json(new { data = existeMarca });
        }

        #endregion
    }
}
