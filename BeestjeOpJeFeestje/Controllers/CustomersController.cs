using BeestjeOpJeFeestje.Data.DatabaseModels;
using Microsoft.AspNetCore.Mvc;
using BeestjeOpJeFeestje.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

[Authorize(Roles ="Admin")]
public class CustomersController : Controller
{
    private readonly ICustomerRepository _customerRepository;

    public CustomersController(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }
    public async Task<IActionResult> Index()
    {
        var animals = await _customerRepository.GetAllAsync();
        return View(animals);
    }

    public IActionResult Create()
    {
        var customerCards = Enum.GetValues(typeof(CustomerCard))
                             .Cast<CustomerCard>()
                             .Select(c => c.ToString())
                             .ToList();
        ViewBag.CustomerCards = customerCards;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Customer customer)
    {
        if (ModelState.IsValid)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = customer.Email,
                Email = customer.Email
            };
            var password = generatePassword();
            await _customerRepository.AddCustomerAndIdentityAsync(customer, user, password);
            TempData["MailAdress"] = customer.Email;
            TempData["GeneratedPassword"] = password;
            return RedirectToAction(nameof(Create));
        }
    
        return View(customer);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        var customerCards = Enum.GetValues(typeof(CustomerCard))
                             .Cast<CustomerCard>()
                             .Select(c => c.ToString())
                             .ToList();
        ViewBag.CustomerCards = customerCards;

        if (customer == null) return NotFound();
        return View(customer);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Customer customer)
    {
        if (ModelState.IsValid)
        {
            await _customerRepository.UpdateAsync(customer);
            return RedirectToAction(nameof(Index));
        }
        return View(customer);
    }

    public async Task<IActionResult> Details(int id)
    {
        var customer = await _customerRepository.GetCustomerCompleteAsync(id);
        if (customer == null) return NotFound();
        return View(customer);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null) return NotFound();
        return View(customer);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _customerRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private string generatePassword()
    { 
        var password = Guid.NewGuid().ToString().Substring(0, 8);
        return password;
    }
}
