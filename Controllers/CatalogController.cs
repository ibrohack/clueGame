using Microsoft.AspNetCore.Mvc;
using clueGame.Services;

namespace clueGame.Controllers;

public class CatalogController : Controller
{
    private readonly MongoDbService _mongo;

    public CatalogController(MongoDbService mongo) => _mongo = mongo;

    public async Task<IActionResult> Character(string id)
    {
        var character = await _mongo.GetCharacterByIdAsync(id);
        return character is null ? NotFound() : View(character);
    }

    public async Task<IActionResult> Weapon(string id)
    {
        var weapon = await _mongo.GetWeaponByIdAsync(id);
        return weapon is null ? NotFound() : View(weapon);
    }

    public async Task<IActionResult> Location(string id)
    {
        var location = await _mongo.GetLocationByIdAsync(id);
        return location is null ? NotFound() : View(location);
    }
}
