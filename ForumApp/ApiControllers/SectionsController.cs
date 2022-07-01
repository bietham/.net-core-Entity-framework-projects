using ForumApp.DTOModels;
using ForumApp.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForumApp.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectionsController : ControllerBase
    {
        private readonly IForumSectionService _forumSection;

        public SectionsController(IForumSectionService forumSection)
        {
            _forumSection = forumSection;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var sections = await _forumSection.GetAllSections();
                return Ok(sections);
            }
            catch
            {
                return StatusCode(500);
            }

        }

        [HttpPost]
        public async Task<IActionResult> Post(ForumSectionCreateDto model)
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
                await _forumSection.PostSection(model);
                return Ok();
            }
            catch (ArgumentException aex)
            {
                return StatusCode(500, aex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Put(ForumSectionEditDto model, int id)
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
                await _forumSection.EditSection(model, id);
                return Ok();
            }
            catch (KeyNotFoundException kex)
            {
                // Logs
                return StatusCode(404, kex.Message);
            }
            catch (ArgumentException aex)
            {
                return StatusCode(500, aex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
