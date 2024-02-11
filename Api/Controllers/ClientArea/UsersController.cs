using Application.Users.Commands;
using Application.Users.Queries;
using Common.Configurations;
using Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Api.Controllers.ClientArea
{
    public class UsersController : BaseController
    {
        public UsersController(IOptions<ApplicationConfiguration> options) : base(options) { }

        [Produces("application/json")]
        [ProducesResponseType(typeof(CreateUserResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
        {
            try
            {
                command.CurrentUser = CurrentUsername;
                command.BrokerId = CurrentBrokerId;

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

        [Produces("application/json")]
        [ProducesResponseType(typeof(ListUsersResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] ListUsersQuery query)
        {
            try
            {
                if (CurrentBrokerId.HasValue)
                {
                    query.BrokerId = CurrentBrokerId;
                }

                var response = await Mediator.Send(query);
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

        [Produces("application/json")]
        [ProducesResponseType(typeof(ReinviteUserResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [HttpPost("reinvite")]
        public async Task<IActionResult> Reinvite([FromBody] ReinviteUserCommand command)
        {
            try
            {
                command.CurrentUser = CurrentUsername;
                command.BrokerId = CurrentBrokerId.HasValue ? CurrentBrokerId : command.BrokerId;
                var response = await Mediator.Send(command);
                return Ok(response);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
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

        [Produces("application/json")]
        [ProducesResponseType(typeof(UpdateUserResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UpdateUserCommand command)
        {
            try
            {
                command.Id = id;
                command.CurrentUser = CurrentUsername;
                command.BrokerId = CurrentBrokerId;

                var response = await Mediator.Send(command);
                return Ok(response);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
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

        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id, [FromQuery] long? brokerId)
        {
            try
            {
                var command = new DeleteUserCommand
                {
                    Id = id,
                    BrokerId = CurrentBrokerId.HasValue ? CurrentBrokerId : brokerId
                };

                await Mediator.Send(command);
                return Ok();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
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
