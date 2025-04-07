using BeestjeOpJeFeestje.Data;
using BeestjeOpJeFeestje.Data.DatabaseModels;
using BeestjeOpJeFeestje.Data.Repositories;
using BeestjeOpJeFeestje.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Security.Claims;

namespace BeestjeOpJeFeestje.Controllers
{
    public class BookingsController : Controller
    {
        private readonly IAnimalRepository _animalRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IBookingService _bookingService;
        public BookingsController(IAnimalRepository animalRepository, IBookingRepository bookingRepository, ICustomerRepository customerRepository, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IBookingService bookingService)
        {
            _animalRepository = animalRepository;
            _bookingRepository = bookingRepository;
            _customerRepository = customerRepository;
            _signInManager = signInManager;
            _userManager = userManager;
            _bookingService = bookingService;
        }

        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> Index()
        {
            List<Booking> bookings = new List<Booking>();
           
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user!);

            if (roles.Contains("Customer"))
            {
                var customer = await _customerRepository.GetCustomerByUserIdAsync(user!.Id);
                bookings = await _bookingRepository.GetBookingsByCustomerIdAsync(customer.Id);
            }
            else if (roles.Contains("Admin"))
            {
                bookings = (await _bookingRepository.GetAllAsync()).ToList();
            }
            

            return View(bookings);
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var customer = await _customerRepository.GetCustomerByUserIdAsync(user!.Id);

            if (customer == null)
            {
                return Unauthorized();
            }

            var booking = await _bookingRepository.GetByIdAsync(id);

            if (booking == null || booking.CustomerId != customer.Id)
            {
                return NotFound();
            }

            await _bookingRepository.DeleteAsync(id);

            TempData["SuccessMessage"] = "Boeking succesvol verwijderd.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> SelectAnimals(DateTime date)
        {
            var allAnimals = await _animalRepository.GetAllAsync();
            var bookedAnimals = await _bookingRepository.GetBookedAnimalsByDateAsync(date);

            var viewModel = new SelectAnimalsViewModel
            {
                SelectedDate = date,
                Animals = allAnimals.Select(a => new AnimalSelectionViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    Type = a.Type,
                    Price = a.Price,
                    ImageUrl = a.ImageUrl,
                    IsBooked = bookedAnimals.Any(b => b.Id == a.Id),
                    IsSelected = false
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmAnimals(SelectAnimalsViewModel viewModel)
        {
            var selectedAnimals = viewModel.Animals.Where(a => a.IsSelected).Select(a => a.Id).ToList();

            var booking = new BookingViewModel
            {
                SelectedDate = viewModel.SelectedDate,
                AnimalIds = selectedAnimals
            }; 

            booking.animals = await _animalRepository.GetAnimalsByIdsAsync(selectedAnimals);
            
            Customer customer = new Customer();
            if (User?.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                customer = await _customerRepository.GetCustomerByUserIdAsync(user!.Id);
            }

            var restrictions = _bookingService.GetAnimalRestrictions(booking, customer);

            if (restrictions.Count > 0)
            {
                TempData["Restrictions"] = JsonConvert.SerializeObject(restrictions);
                return RedirectToAction("SelectAnimals", viewModel.SelectedDate );
            }

            TempData["Booking"] = JsonConvert.SerializeObject(booking);

            return View("PersonalDataOrLogin", null);
        }

        [HttpGet]
        public IActionResult PersonalDataOrLogin(CombinedPDViewModel? _model)
        {
            var model = _model == null ? new CombinedPDViewModel() : _model;
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> ConfirmBooking()
        {
            var bookingJson = TempData["Booking"] as string;
            var customerJson = TempData["Customer"] as string;
            if (string.IsNullOrEmpty(bookingJson) || string.IsNullOrEmpty(customerJson))
            {
                return RedirectToAction("SelectAnimals");
            }
            var bookingViewModel = JsonConvert.DeserializeObject<BookingViewModel>(bookingJson);
            var customer = JsonConvert.DeserializeObject<Customer>(customerJson);

            Booking booking = new Booking
            {
                Date = bookingViewModel!.SelectedDate,
                CustomerId = customer!.Id,
                Customer = customer,
                Animals = bookingViewModel.animals,
                TotalPrice = bookingViewModel.TotalPrice,
                IsConfirmed = true
            };
            await _bookingRepository.AddAsync(booking);
            TempData.Remove("Booking");
            TempData["success"] = "success";

        
            return RedirectToAction("index", "home");
        }

        [HttpPost]
        public async Task<IActionResult> SubmitPersonalData(CombinedPDViewModel model)
        {
            BookingViewModel booking = new();
            Customer customer = new();

            if (User?.Identity?.IsAuthenticated == false)
            {
                if (!ModelState.IsValid)
                {
                    return View("PersonalDataOrLogin", model); // Geef het formulier opnieuw weer bij fouten
                } 
            }

            // Probeer booking uit TempData te halen
            var bookingJson = TempData["Booking"] as string;
            if (!string.IsNullOrEmpty(bookingJson))
            {
                booking = JsonConvert.DeserializeObject<BookingViewModel>(bookingJson);
                TempData.Keep("Booking");
            }
            else
            {
                TempData["ErrorMessage"] = "Er is iets fout gegaan, probeer het opnieuw";
                return RedirectToAction("Index", "Home");
            }
            

            // Check of de gebruiker is ingelogd
            if (User?.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                customer = await _customerRepository.GetCustomerByUserIdAsync(user?.Id);

                if (customer != null)
                {
                    model.PersonalDataViewModel.Name = customer.Name;
                    model.PersonalDataViewModel.PhoneNumber = customer.PhoneNumber;
                    model.PersonalDataViewModel.Email = customer.Email;
                }
            }

            // Als er geen ingelogde klant is, maak een nieuwe klant aan
            if (customer == null)
            {
                customer = new Customer
                {
                    Name = model.PersonalDataViewModel.Name,
                    PhoneNumber = model.PersonalDataViewModel.PhoneNumber,
                    Email = model.PersonalDataViewModel.Email,
                    Address = model.PersonalDataViewModel.Address,
                    CustomerCard = CustomerCard.None
                };
                await _customerRepository.AddCustomerAsync(customer);
            }

            booking.PersonalData = model.PersonalDataViewModel;
            booking.animals = await _animalRepository.GetAnimalsByIdsAsync(booking.AnimalIds);

            // Bereken de korting en totaalprijs
            (booking.TotalPrice, booking.Discounts) = _bookingService.CalculateDiscount(booking, customer);

            TempData["Booking"] = JsonConvert.SerializeObject(booking);
            TempData["Customer"] = JsonConvert.SerializeObject(customer);
            return View("ConfirmBooking", booking);
        }


        [HttpPost]
        public async Task<IActionResult> Login(CombinedPDViewModel model)
        {
            if(model.LoginViewModel.Email.IsNullOrEmpty() || model.LoginViewModel.Password.IsNullOrEmpty())
            {
                return RedirectToAction("PersonalDataOrLogin", model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.LoginViewModel.Email, model.LoginViewModel.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return RedirectToAction("PersonalDataOrLogin");
            }
           
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View("PersonalDataOrLogin", model);
        }
    }
}
