using Api.Controllers.ClientArea;
using Application.Accounts.Commands;
using Common.Configurations;
using Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Api.Controllers
{
    public class AccountsController : BaseController
    {
        public AccountsController(IOptions<ApplicationConfiguration> options) : base(options) { }

        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(AccountLoginResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AccountLoginCommand command)
        {
            try
            {
                command.Subdomain = Subdomain;
                var response = await Mediator.Send(command);
                return Ok(response);
            }
            catch (NotFoundException)
            {
                return BadRequest("Invalid username or password.");
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(AccountActivateResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [HttpPost("activate")]
        public async Task<IActionResult> Activate([FromBody] AccountActivateCommand command)
        {
            try
            {
                command.Subdomain = Subdomain;
                command.ClientUrl = ClientUrl;
                var response = await Mediator.Send(command);
                return Ok(response);
            }
            catch (NotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(RequestResetPasswordResponse), 200)]
        [HttpPost("request-reset-password")]
        public async Task<IActionResult> RequestResetPassword([FromBody] RequestResetPasswordCommand command)
        {
            try
            {
                command.Subdomain = Subdomain;
                command.ClientUrl = ClientUrl;
                var response = await Mediator.Send(command);
                return Ok(response);
            }
            catch (Exception)
            {
                // Should always return 200
                return Ok(new RequestResetPasswordResponse());
            }
        }

        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ResetPasswordResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            try
            {
                command.Subdomain = Subdomain;
                command.ClientUrl = ClientUrl;
                var response = await Mediator.Send(command);
                return Ok(response);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [Authorize]
        [Produces("application/json")]
        [ProducesResponseType(typeof(RefreshTokenResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [HttpGet("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                var refresh = new RefreshTokenCommand
                {
                    UserId = CurrentUserId
                };
                var response = await Mediator.Send(refresh);
                return Ok(response);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}
