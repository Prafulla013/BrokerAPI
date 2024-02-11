using Api.Controllers.ClientArea;
using Application.Files.Commands;
using Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Api.Controllers
{
    public class FilesController : BaseController
    {
        [Produces("application/json")]
        [ProducesResponseType(typeof(UploadFileCommand), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] UploadFileCommand command)
        {
            try
            {
                var response = await Mediator.Send(command);
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Produces("application/json")]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [HttpGet("download")]
        public async Task<IActionResult> Download([FromQuery] GetFileQuery query)
        {
            try
            {
                var response = await Mediator.Send(query);
                var file = File(response.FileBytes, response.ContentType, response.FileName);
                return file;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
