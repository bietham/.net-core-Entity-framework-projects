
using ForumApp.DTOModels;
using ForumApp.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace ForumApp.ApiControllers
{
    
    [ApiController]
    public class TopicsController : ControllerBase
    {
        private readonly ITopicService _topic;

        public TopicsController(ITopicService topic)
        {
            _topic = topic;
        }

        [HttpGet]
        [Route("api/sections/{id}/topics")]
        public async Task<IActionResult> Get(int Id)
        {
            try
            {
                var sections = await _topic.GetAllTopics(Id);
                return Ok(sections);
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

        [HttpPost]
        [Route("api/sections/{id}/topics")]
        public async Task<IActionResult> Post(TopicCreateDto model, int Id)
        {
            if (model == null)
            {
                return BadRequest("There must be an object, not null");
            }

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return BadRequest("Title must be filled");
            }
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return BadRequest("Description must be added");
            }

            try
            {
                await _topic.PostTopic(model, Id);
                return Ok();
            }
            catch (ArgumentException aex)
            {
                return StatusCode(400, aex.Message);
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
        [Route("api/topics/{id}")]
        public async Task<IActionResult> Put(TopicEditDto model, int id)
        {
            if (model == null)
            {
                return BadRequest("There must be an object, not null");
            }

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return BadRequest("Title must be filled");
            }
            if (string.IsNullOrWhiteSpace(model.Description))
            {
                return BadRequest("Description must be filled");
            }

            try
            {
                await _topic.EditTopic(model, id);
                return Ok();
            }
            catch (KeyNotFoundException kex)
            {
                return StatusCode(404, kex.Message);
            }

            catch (ArgumentException aex)
            {
                return StatusCode(400, aex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        [Route("api/topics/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _topic.DeleteTopic(id);
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
        [HttpGet]
        [Route("api/topics/{id}/messages")]
        public async Task<IActionResult> GetMessage(int id)
        {
            try
            {
                var messages = await _topic.GetTopicMessages(id);
                return Ok(messages);
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
