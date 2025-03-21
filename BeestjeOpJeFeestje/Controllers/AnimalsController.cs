using BeestjeOpJeFeestje.Data.DatabaseModels;
using Microsoft.AspNetCore.Mvc;
using BeestjeOpJeFeestje.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

[Authorize(Roles ="Admin")]
public class AnimalsController : Controller
{
    private readonly IAnimalRepository _animalRepository;

    public AnimalsController(IAnimalRepository animalRepository)
    {
        _animalRepository = animalRepository;
    }
    public async Task<IActionResult> Index()
    {
        var animals = await _animalRepository.GetAllAsync();
        return View(animals);
    }

    public IActionResult Create()
    {
        var categories = Enum.GetValues(typeof(AnimalType))
                             .Cast<AnimalType>()
                             .Select(c => c.ToString())
                             .ToList();
        ViewBag.Categories = categories;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Animal animal)
    {
        if (ModelState.IsValid)
        {
            await _animalRepository.AddAsync(animal);
            return RedirectToAction(nameof(Index));
        }
        return View(animal);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var animal = await _animalRepository.GetByIdAsync(id);
        var categories = Enum.GetValues(typeof(AnimalType))
                             .Cast<AnimalType>()
                             .Select(c => c.ToString())
                             .ToList();
        ViewBag.Categories = categories;

        if (animal == null) return NotFound();
        return View(animal);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Animal animal)
    {
        if (ModelState.IsValid)
        {
            await _animalRepository.UpdateAsync(animal);
            return RedirectToAction(nameof(Index));
        }
        return View(animal);
    }

    public async Task<IActionResult> Details(int id)
    {
        var animal = await _animalRepository.GetAnimalCompleteAsync(id);
        if (animal == null) return NotFound();
        return View(animal);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var animal = await _animalRepository.GetByIdAsync(id);
        if (animal == null) return NotFound();
        return View(animal);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _animalRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
