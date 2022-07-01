using ForumApp.DTOModels;
using ForumApp.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForumApp.ApiControllers
{
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _message;

        public MessagesController(IMessageService message)
        {
            _message = message;
        }


        [HttpPost]
        [Route("api/topics/{id}/messages")]
        public async Task<IActionResult> Post(MessageEditDto model, int Id)
        {
            if (model == null)
            {
                return BadRequest("There must be an object, not null");
            }

            if (string.IsNullOrWhiteSpace(model.Text))
            {
                return BadRequest("Message cannot be empty");
            }
            

            try
            {
                await _message.PostMessage(model, Id);
                return Ok();
            }
            catch (KeyNotFoundException kex)
            {
                return StatusCode(404, kex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        [Route("api/messages/{id}")]
        public async Task<IActionResult> Put(MessageEditDto model, int id)
        {
            if (model == null)
            {
                return BadRequest("There must be an object, not null");
            }
            if (string.IsNullOrWhiteSpace(model.Text))
            {
                return BadRequest("Message cannot be empty");
            }

            try
            {
                await _message.EditMessage(model, id);
                return Ok();
            }
            catch (KeyNotFoundException kex)
            {
                return StatusCode(404, kex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        [Route("api/messages/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _message.DeleteMessage(id);
                return Ok();
            }
            catch (KeyNotFoundException kex)
            {
                return StatusCode(404, kex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
    }
}
