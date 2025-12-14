using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Services.Interfaces;
using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationApi.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthenticationController(IUserService userService) : ControllerBase
{
    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<GetUserDTO>> GetUser(int id)
    {
        if (id <= 0)
            return BadRequest("Invalid user id");

        var result = await userService.GetUserAsync(id);

        return result!= null &&result.Id >= 0 ? Ok(result) : NotFound();
    }


    [HttpPost("register")]
    public async Task<ActionResult<Response>> Register(AppUserDTO appUserDTO)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await userService.RegisterAsync(appUserDTO);

        return result.Flag ? Ok(result) : BadRequest(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<Response>> Login(LoginDTO loginDTO)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await userService.LoginAsync(loginDTO);

        return result.Flag ? Ok(result) : BadRequest(result);
    }
}
