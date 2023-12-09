using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DataAcess.Data;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
/*using Microsoft.CodeAnalysis.VisualBasic.Syntax;*/


namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
   /* [Authorize(Roles = SD.Role_Admin)]*/
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
     
        public CompanyController(IUnitOfWork unitOfWork, IWebHostEnvironment webhostEnvironment)
        {
            _unitOfWork = unitOfWork;
           
        }
        public IActionResult Index()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();

            return View(objCompanyList);
        }

        public IActionResult Upsert(int? id)
        {
            
            if(id ==null || id == 0)
            {
                //this is for create
                return View(new Company());
            } 
            else {
                //this is for edit
                Company companyObj = _unitOfWork.Company.Get(u=>u.Id==id);
                return View(companyObj);
            }
        }
        [HttpPost]
        public IActionResult Upsert(Company CompanyObj)
        {
            if (ModelState.IsValid)
            {

              
                //check for if we want to update or add if id present update else add
                if (CompanyObj.Id == 0)
                {
                    //add
                    _unitOfWork.Company.Add(CompanyObj);
                }
                else 
                {
                    //update 
                    _unitOfWork.Company.Update(CompanyObj);
                }
                
                _unitOfWork.Save();
                TempData["success"] = "Company created successfully";
                return RedirectToAction("Index");
            }
            else
            {
               
                return View(CompanyObj);
            }
        }

       
        
        #region API call 
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return Json( new { data = objCompanyList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var CompanyToBeDeleted = _unitOfWork.Company.Get(u => u.Id == id);
            if (CompanyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

           

            _unitOfWork.Company.Remove(CompanyToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion
    }
}