using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampsController : ControllerBase
    {
        private readonly ICampRepository _campRepository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator linkGenerator;

        public CampsController(ICampRepository campRepository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _campRepository = campRepository;
            _mapper = mapper;
            this.linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<CampModel[]>> GetCamps(bool includeTalks = false)
        {
            try
            {
                var camps = await _campRepository.GetAllCampsAsync(includeTalks);

                return Ok(_mapper.Map<CampModel[]>(camps)); //if list of CampModels are present, will automatically return Status Code 200 OK
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
        }

        [HttpGet("{moniker}")] // route parameter assigned to the variable 'moniker' (e.g. www.host.com/api/camps/ATL2018, moniker = "ATL2018")
        public async Task<ActionResult<CampModel>> GetCamp(string moniker)
        {
            try
            {
                var camp = await _campRepository.GetCampAsync(moniker);

                if (camp == null) { return NotFound(); }

                return Ok(_mapper.Map<CampModel>(camp));
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
        }

        [HttpGet("search")] // string literal 'search' route parameter (e.g. www.host.com/api/camps/search?date=2023-12-12&includeTalks=true)
        public async Task<ActionResult<CampModel[]>> GetCampsByDate(DateTime date, bool includeTalks = false)
        {
            try
            {
                var camps = await _campRepository.GetAllCampsByEventDate(date, includeTalks);

                if (!camps.Any()) { return NotFound(); }

                return Ok(_mapper.Map<CampModel[]>(camps));
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
        }

        [HttpPost]
        public async Task<ActionResult<CampModel>> PostCamp(CampModel newCampModel)
        {
            try
            {
                //validate that the camp moniker is unique
                var existingCamp = await _campRepository.GetCampAsync(newCampModel.Moniker);
                if (existingCamp != null) { return BadRequest("Moniker is in use."); }

                // create link to created resource
                var location = linkGenerator.GetPathByAction("GetCamp", "Camps", new { moniker = newCampModel.Moniker });
                if (string.IsNullOrEmpty(location)) { return BadRequest("Could not use the provided moniker."); }

                // create new camp and save to database
                var newCampEntity = _mapper.Map<Camp>(newCampModel);
                _campRepository.Add(newCampEntity);

                if (await _campRepository.SaveChangesAsync())
                {
                    return Created(location, _mapper.Map<CampModel>(newCampEntity));
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }

            return BadRequest();
        }

        [HttpPut("{moniker}")]
        public async Task<ActionResult<CampModel>> PutCamp(string moniker, CampModel newCampModel)
        {
            try
            {
                // checking if new moniker already exists
                if (moniker != newCampModel.Moniker)
                {
                    var existingMoniker = await _campRepository.GetCampAsync(newCampModel.Moniker);
                    if (existingMoniker != null)
                    {
                        return BadRequest("The updated moniker already exists.");
                    }
                }

                var existingCampEntity = await _campRepository.GetCampAsync(moniker);
                if (existingCampEntity == null) 
                {
                    return NotFound($"Could not find camp with the given moniker {moniker}."); 
                }

                _mapper.Map(newCampModel, existingCampEntity);

                if (await _campRepository.SaveChangesAsync())
                {
                    return Ok(_mapper.Map<CampModel>(existingCampEntity));
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }

            return BadRequest();
        }

        [HttpDelete("{moniker}")]
        public async Task<IActionResult> DeleteCamp(string moniker)
        {
            try
            {
                var existingCamp = await _campRepository.GetCampAsync(moniker, true);
                if (existingCamp == null) { return NotFound(); }

                _campRepository.Delete(existingCamp);

                // also delete talks that references the camp we are attempting to delete
                

                if (await _campRepository.SaveChangesAsync())
                {
                    return Ok();
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }

            return BadRequest("Failed to delete camp.");
        }
    }
}
