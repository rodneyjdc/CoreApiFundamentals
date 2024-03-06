using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [Route("api/camps/{moniker}/[controller]")]
    [ApiController]
    public class TalksController : ControllerBase
    {
        private readonly ICampRepository _campRepository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        public TalksController(ICampRepository campRepository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _campRepository = campRepository;
            _mapper = mapper;   
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<TalkModel[]>> GetTalks(string moniker)
        {
            try
            {
                var talks = await _campRepository.GetTalksByMonikerAsync(moniker, true);
                if (talks == null)
                {
                    return NotFound("Talks could not be found.");
                }

                return Ok(_mapper.Map<TalkModel[]>(talks));
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TalkModel>> GetTalk(string moniker, int id)
        {
            try
            {
                var talk = await _campRepository.GetTalkByMonikerAsync(moniker, id, true);
                if (talk == null)
                {
                    return NotFound("Talk could not be found.");
                }

                return Ok(_mapper.Map<TalkModel>(talk));
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
        }

        [HttpPost]
        public async Task<ActionResult<TalkModel>> PostTalk(string moniker, TalkModel talkModel)
        {
            try
            {
                var camp = await _campRepository.GetCampAsync(moniker);
                if (camp == null) { return BadRequest("Camp not found."); }

                if (talkModel.Speaker == null)
                {
                    return BadRequest("Talk must include a speaker ID");
                }

                var speaker = await _campRepository.GetSpeakerAsync(talkModel.Speaker.SpeakerId);
                if (speaker == null) { return BadRequest("Speaker not found."); }

                var talkEntity = _mapper.Map<Talk>(talkModel);
                talkEntity.Speaker = speaker;
                talkEntity.Camp = camp;

                _campRepository.Add(talkEntity);

                if (await _campRepository.SaveChangesAsync()) // SaveChangesAsync() automatically sets the entity IDs!!!!
                {
                    var url = _linkGenerator.GetPathByAction(HttpContext,
                        "GetTalk",
                        values: new { moniker, id = talkEntity.TalkId });

                    return Created(url, _mapper.Map<TalkModel>(talkEntity));
                }

                return BadRequest("Failed to save new talk.");
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
        }

        [HttpPut("{talkId:int}")]
        public async Task<ActionResult<TalkModel>> PutTalk(string moniker, int talkId, TalkModel talkModel)
        {
            try
            {
                var talk = await _campRepository.GetTalkByMonikerAsync(moniker, talkId);
                if (talk == null) { return NotFound("Could not find talk."); }

                _mapper.Map(talkModel, talk);

                if (talkModel.Speaker != null) 
                {
                    var speaker = await _campRepository.GetSpeakerAsync(talkModel.Speaker.SpeakerId);
                    if (speaker != null)
                    {
                        talk.Speaker = speaker;
                    }
                }

                if (await _campRepository.SaveChangesAsync()) 
                {
                    return _mapper.Map<TalkModel>(talk);
                }

                return BadRequest("Failed to update database");
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
        }

        [HttpDelete("{talkId:int}")]
        public async Task<IActionResult> DeleteTalk(string moniker, int talkId)
        {
            try
            {
                var talkEntity = await _campRepository.GetTalkByMonikerAsync(moniker, talkId);
                if (talkEntity == null) { return NotFound("Talk could not be found."); }

                _campRepository.Delete(talkEntity);

                if (await _campRepository.SaveChangesAsync())
                {
                    return Ok();
                }

                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
        }
    }
}
