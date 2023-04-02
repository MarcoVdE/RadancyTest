
namespace RadancyBankingSystemTest.Api.Controllers;

using Domain.Dto.Exceptions;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Domain.Interfaces;
using Domain.Models.Exceptions;
using Domain.Dto.Models;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserAccountService _userAccountService;
    private readonly ILogger<UsersController> _logger;
    private readonly IMapper _mapper;

    public UsersController(
        IUserAccountService userAccountService,
        IMapper mapper,
        ILogger<UsersController> logger)
    {
        _userAccountService = userAccountService;
        _mapper = mapper;
        _logger = logger;
    }
    
    /// <summary>
    /// Create a User account
    /// </summary>
    /// <param name="idDocument"></param>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    // Usually for account creation, this would be quite a few more params, so you'd use a CreateUserDto/request.
    public async Task<IActionResult> CreateUser(string idDocument, string firstName, string lastName)
    {
        try
        {
            var userId = await _userAccountService.CreateUser(idDocument, firstName, lastName);
            return Ok(userId);
        }
        catch (UserException e)
        {
            var response = _mapper.Map<UserExceptionDto>(e);
            return new ObjectResult(response) { StatusCode = response.StatusCode };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while creating user");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Get a user
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("{userId:Guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(KeyNotFoundException), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUser(Guid userId)
    {
        // Usually you'd use a token system and get the userId from the access token, otherwise direct allow get account.
        try
        {
            var user = await _userAccountService.GetUser(userId);
            var response = _mapper.Map<UserDto>(user);
            return Ok(response);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Could not find the user (UserId: {userId})");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting user (UserId: {UserId})", userId);
            return StatusCode(500);
        }
    }
}