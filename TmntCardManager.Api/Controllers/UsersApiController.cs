using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmntCardManager.Api.DTOs;
using TmntCardManager.Models;

namespace TmntCardManager.Api.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
[ApiController]
public class UsersApiController : ControllerBase
{
    private readonly UserManager<User> _userManager;

    public UsersApiController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserOutputDto>>> GetUsers() 
    {
        var users = await _userManager.Users
            .Select(u => new UserOutputDto 
            {
                Id = u.Id,
                Email = u.Email
            })
            .ToListAsync();
            
        return Ok(users);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return NotFound();
        await _userManager.DeleteAsync(user);
        return Ok(new { message = "Користувача видалено" });
    }
}