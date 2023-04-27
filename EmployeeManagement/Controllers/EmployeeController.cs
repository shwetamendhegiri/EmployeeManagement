using EmployeeManagement.Data;

using EmployeeManagement.Models;
using EmployeeManagement.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext applicationDbContext;

        public EmployeeController(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
           var employees = await applicationDbContext.Employees.ToListAsync();
            return View(employees);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(AddEmployeeViewModel addEmployeeRequest) 
        {
            var employee = new Employee()
            {
                Id = Guid.NewGuid(),
                Name = addEmployeeRequest.Name,
                Email = addEmployeeRequest.Email,
                PhoneNumber = addEmployeeRequest.PhoneNumber

            };
            applicationDbContext.Employees.AddAsync(employee);
            applicationDbContext.SaveChangesAsync();
            return RedirectToAction("Add");
        }

        [HttpGet]
        public async Task<IActionResult> View(Guid id)
        { 
            var employee = await applicationDbContext.Employees.FirstOrDefaultAsync(x => x.Id == id);

            if (employee != null)
            {

                var viewModel = new UpdateEmployeeViewModel()
                {
                    Id = employee.Id,
                    Name = employee.Name,
                    Email = employee.Email,
                    PhoneNumber = employee.PhoneNumber

                };
                return await Task.Run(()=> View("view",viewModel));
            }
            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> View(UpdateEmployeeViewModel model)
        {
            var employee = await applicationDbContext.Employees.FindAsync(model.Id);

            if (employee != null)
            {
                employee.Name= model.Name;
                employee.Email= model.Email;
                employee.PhoneNumber= model.PhoneNumber;

                await applicationDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(UpdateEmployeeViewModel model)
        {
            var employee = await applicationDbContext.Employees.FindAsync(model.Id);
            if(employee != null)
            {
                applicationDbContext.Employees.Remove(employee);
                await applicationDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}
