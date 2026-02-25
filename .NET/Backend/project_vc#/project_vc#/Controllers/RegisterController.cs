using Microsoft.AspNetCore.Mvc;
using project_vc_.Models;
using project_vc_.Services;

namespace project_vc_.Controllers;

[ApiController]
[Route("api/registration")]
public class RegisterController : ControllerBase
{
    private readonly IUserService _service;

    public RegisterController(IUserService service)
    {
        _service = service;
    }

    [HttpGet]
    public ActionResult<List<User>> GetAll()
    {
        return Ok(_service.GetAllRegistrations());
    }

    [HttpPost]
    public ActionResult<User> Save([FromBody] User user)
    {
        try
        {
            var saved = _service.SaveRegistration(user);
            return Ok(saved);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
