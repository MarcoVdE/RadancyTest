
namespace RadancyBankingSystemTest.Api.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Domain.Interfaces;
using Domain.Models.Exceptions;
using Domain.Dto.Exceptions;
using Domain.Dto.Models;

[Route("api/users/{userId:Guid}/[controller]/")]
[ApiController]
public class AccountsController : ControllerBase
{
    private readonly IUserAccountService _userAccountService;
    private readonly IMapper _mapper;
    private readonly ILogger<AccountsController> _logger;

    public AccountsController(IUserAccountService userAccountService, IMapper mapper,
        ILogger<AccountsController> logger)
    {
        _userAccountService = userAccountService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost("")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(KeyNotFoundException), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateAccount(Guid userId)
    {
        // Usually you'd use a token system and get the userId from the access token, otherwise direct allow get account.
        //   I chose middle ground as "mock" as though you'd get from token.
        try
        {
            var accountId = await _userAccountService.CreateAccount(userId);
            return Ok(accountId);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Could not create an account for the user ({userId})");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while creating user account (User: {UserId})", userId);
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Get a user's account
    /// Note to format to 2 character decimal, floor the value
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="accountId"></param>
    /// <returns></returns>
    [HttpGet("{accountId:Guid}")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(KeyNotFoundException), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAccount(Guid userId, Guid accountId)
    {
        // Usually you'd use a token system and get the userId from the access token, otherwise direct allow get account.
        //   I chose middle ground as "mock" as though you'd get from token.
        try
        {
            var account = await _userAccountService.GetUserAccount(userId, accountId);
            return Ok(account);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Could not find an account with the given id ({accountId}) for the user ({userId})");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting user account (User: {UserId}, Account: {AccountId})", userId,
                accountId);
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Withdraw or deposit from a user's account.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="accountId"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    [HttpPatch("{accountId:Guid}")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(KeyNotFoundException), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAccount(Guid userId, Guid accountId, decimal amount)
    {
        // Usually you'd use a token system and get the userId from the access token, otherwise direct allow get account.
        //   I chose middle ground as "mock" as though you'd get from token.
        try
        {
            var account = await _userAccountService.UpdateAccount(userId, accountId, amount);
            var response = _mapper.Map<AccountDto>(account);
            return Ok(response);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Could not find an account with the given id ({accountId}) for the user ({userId})");
        }
        catch (AccountException e)
        {
            var response = _mapper.Map<AccountExceptionDto>(e);
            return new ObjectResult(response) { StatusCode = response.StatusCode };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting user account (User: {UserId}, Account: {AccountId})", userId,
                accountId);
            return StatusCode(500);
        }
    }
 
    /// <summary>
    /// Delete a user's account.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="accountId"></param>
    /// <returns></returns>
    [HttpDelete("{accountId:Guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult DeleteAccount(Guid userId, Guid accountId)
    {
        // 200/404 is all "valid", no need to wait / fire and forget 
        _userAccountService.DeleteAccount(userId, accountId);
        return Ok();
    }
}