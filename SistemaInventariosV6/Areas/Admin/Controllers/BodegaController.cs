﻿using Microsoft.AspNetCore.Mvc;
using SistemaInventarios.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventarios.Modelos;
using SistemaInventarios.Utilidades;

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

        //Este metodo es de tipo GET
        public async Task<IActionResult> Upsert(int? id)
        {
            Bodega bodega = new Bodega();

            if(!id.HasValue)
            {
                //Crear una nueva bodega
                bodega.Estado = true;
                return View(bodega);
            }
            
            //Se actualiza bodega
            bodega = await _unidadTrabajo.Bodega.Obtener(id.GetValueOrDefault());
            if (bodega == null)
                return NotFound();

            return View(bodega);
        }

        [HttpPost] //hay que indicar esta etiqueta al metodo para indicar que es de tipo POST
        [ValidateAntiForgeryToken]//esto evita las falsificaciones de solicitudes de otro sitio que está intentando cargar datos en nuestra página
        public async Task<IActionResult> Upsert(Bodega bodega)
        {
            if(ModelState.IsValid) //valida que el modelo este correcto en cada una de sus propiedades
            { 
                if(bodega.Id == 0)
                {
                    await _unidadTrabajo.Bodega.Agregar(bodega);
                    TempData[DS.Exitosa] = "Bodega creada exitosamente";
                }
                else
                {
                    _unidadTrabajo.Bodega.Actualizar(bodega);
                    TempData[DS.Exitosa] = "Bodega actualizada exitosamente";
                }

                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));//redirecciona a Index
            }

            TempData[DS.Error] = "Error al grabar bodega";
            return View(bodega);
        }

        #region API

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()//el "IActionResult" retorna una vista y objetos con formato json
        {
            var todos = await _unidadTrabajo.Bodega.ObtenerTodos();
            return Json(new { data = todos });//"data" es el nombre que se va usar en la javascript, por lo que si se cambia ese nombre aquí, se debe usar ese mismo en el javascript
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            //Se obtiene la bodega según el ID indicado
            var bodegaDB = await _unidadTrabajo.Bodega.Obtener(id);

            //Si no existe la bodega
            if(bodegaDB == null)
            {
                //Se indica por medio de javascript un mensaje indicando el error
                //y se finaliza el proceso
                return Json(new { success = false, message = "Error al borrar bodega" });
            }

            //Si si existe la bodega, se realiza la eliminación
            _unidadTrabajo.Bodega.Remover(bodegaDB);

            //Se guardan los cambios.
            await _unidadTrabajo.Guardar();

            //Se indica un mensaje correspondiente a la eliminación
            return Json(new { success = true, message = "Bodega " + bodegaDB.Nombre + " borrada exitosamente" });
        }

        [ActionName("ValidarNombre")]//se indica este Tag para poder referenciarlo en el javascript desde la vista upsert
        public async Task<IActionResult> ValidarNombre(string nombre, int id=0)
        {
            bool existeBodega = false;
            var lista = await _unidadTrabajo.Bodega.ObtenerTodos();

            //Si es una bodega nueva
            if(id == 0)
            {
                //Se indica si existe la bodega que se quiere registrar
                existeBodega = lista.Any(b=>b.Nombre.ToLower().Trim().Equals(nombre.ToLower().Trim()));
            }
            //pero si es una bodega existente
            else
            {
                //Se indica si existe la bodega que se quiere modificar
                existeBodega = lista.Any(b => b.Nombre.ToLower().Trim().Equals(nombre.ToLower().Trim()) && b.Id != id);
            }

            //Se retorna un json indicando si existe o no la bodega indicada
            return Json(new { data = existeBodega });
        }

        #endregion
    }
}
