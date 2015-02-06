using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DbCrudController
{
    public class GenericDbCludController<TEntity, TDC> : Controller
        where TDC : DbContext, new()
        where TEntity : class,new()
    {
        protected TDC DbContext = new TDC();

        protected virtual IQueryable<TEntity> GetQuery()
        {
            return CurrentEntitys.AsQueryable();
        }

        protected DbSet<TEntity> CurrentEntitys
        {
            get
            {
                return DbContext.Set<TEntity>();
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.HttpMethod.ToLower() == "get")
            {
                if (filterContext.HttpContext.Request.UrlReferrer != null)
                {
                    ViewBag.__ReturnUrl = filterContext.HttpContext.Request.UrlReferrer.ToString();
                }
            }
            else
            {
                ViewBag.__ReturnUrl = filterContext.HttpContext.Request.Form["__ReturnUrl"];
            }

            base.OnActionExecuting(filterContext);
        }

        //
        // GET: /Generic/

        public virtual ActionResult Index()
        {
            var query = this.GetQuery();
            return View(query);
        }

        //
        // GET: /Generic/Details/5

        public virtual ActionResult Details(int id)
        {
            var entity = CurrentEntitys.Find(id);
            return View(entity);
        }

        //
        // GET: /Generic/Create

        public virtual ActionResult Create()
        {

            return View(new TEntity());
        }

        //
        // POST: /Generic/Create

        [HttpPost]
        public virtual ActionResult Create(FormCollection collection)
        {
            var entity = new TEntity();
            TryUpdateModel(entity);
            if (!ModelState.IsValid)
                return View(entity);
            try
            {
                // TODO: Add insert logic here
                CurrentEntitys.Add(entity);
                DbContext.SaveChanges();
                return ReturnPage();
            }
            catch (Exception e)
            {
                var sqlEx = FindSqlException(e);
                if (sqlEx != null)
                {
                    return SqlExceptionProcess(entity, sqlEx);
                }
                ModelState.AddModelError("_", e.ToString());
                return View(entity);
            }
        }

        private SqlException FindSqlException(Exception ex)
        {
            if (ex is SqlException)
                return (SqlException)ex;

            if (ex.InnerException == null)
                return null;

            return FindSqlException(ex.InnerException);

        }

        //
        // GET: /Generic/Edit/5

        public virtual ActionResult Edit(int id)
        {
            var entity = CurrentEntitys.Find(id);
            return View(entity);
        }

        //
        // POST: /Generic/Edit/5

        [HttpPost]
        public virtual ActionResult Edit(int id, FormCollection collection)
        {
            var entity = CurrentEntitys.Find(id);
            TryUpdateModel(entity);
            if (!ModelState.IsValid)
                return View(entity);
            try
            {
                // TODO: Add insert logic here
                DbContext.SaveChanges();
                return ReturnPage();
            }
            catch (Exception e)
            {
                var sqlEx = FindSqlException(e);
                if (sqlEx != null)
                {
                    return SqlExceptionProcess(entity, sqlEx);
                }

                ModelState.AddModelError("_", e.ToString());
                return View(entity);
            }
        }

        protected ActionResult SqlExceptionProcess(TEntity entity, SqlException se)
        {
            var isHandled = SqlExceptionHandlling(se);
            if (!isHandled)
            {
                ModelState.AddModelError("_", se.Message);
            }
            return View(entity);
        }

        //
        // GET: /Generic/Delete/5

        public virtual ActionResult Delete(int id)
        {
            var entity = CurrentEntitys.Find(id);
            return View(entity);
        }

        private ActionResult ReturnPage()
        {
            var returnUrl = Request.Form["__ReturnUrl"];
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        //
        // POST: /Generic/Delete/5

        [HttpPost]
        public virtual ActionResult Delete(int id, FormCollection collection)
        {
            var entity = CurrentEntitys.Find(id);
            try
            {
                // TODO: Add delete logic here
                CurrentEntitys.Remove(entity);
                DbContext.SaveChanges();
                return ReturnPage();

            }
            catch (SqlException se)
            {
                return SqlExceptionProcess(entity, se);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("_", e.ToString());
                return View(entity);
            }
        }

        public virtual bool SqlExceptionHandlling(SqlException sqlex)
        {
            return false;
        }

    }
}
