using ABCretail_CLVD.Models;
using ABCretail_CLVD.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ABCretail_CLVD.Controllers
{
    public class TableStorageController : Controller
    {
        private readonly TableStorageService<CustomerEntity> _customerTableService;

        public TableStorageController(TableStorageService<CustomerEntity> customerTableService)
        {
            _customerTableService = customerTableService;
        }

        // Index action to display the view
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var customerProfiles = await _customerTableService.RetrieveAllEntitiesAsync();
            return View(customerProfiles);
        }

        // GET: Edit view for adding or editing a customer profile
        [HttpGet]
        public async Task<IActionResult> Edit(string partitionKey, string rowKey)
        {
            CustomerEntity customer;

            if (!string.IsNullOrEmpty(partitionKey) && !string.IsNullOrEmpty(rowKey))
            {
                // Editing an existing customer
                customer = await _customerTableService.RetrieveEntityAsync(partitionKey, rowKey);
                if (customer == null)
                {
                    return NotFound("Customer not found.");
                }
            }
            else
            {
                // Adding a new customer
                customer = new CustomerEntity();
            }

            return View(customer);
        }

        // POST: Edit action to save the customer profile (insert or update)
        [HttpPost]
        public async Task<IActionResult> Edit(CustomerEntity customerEntity)
        {
            if (ModelState.IsValid)
            {
                // Insert or update the customer in table storage
                await _customerTableService.InsertOrMergeEntityAsync(customerEntity);
                return RedirectToAction("Index");
            }

            return View(customerEntity);
        }
    }



}
