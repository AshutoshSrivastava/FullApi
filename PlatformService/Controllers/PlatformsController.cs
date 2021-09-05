using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo _repo;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;

        public PlatformsController(IPlatformRepo repo, 
            IMapper mapper,
            ICommandDataClient commandDataClient
        )
        {
            _repo = repo;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
        }
        
        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            Console.WriteLine("--> Getting Platforms");
            var platformItems = _repo.GetAllPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>> (platformItems));
        }

        
        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            var platformItem = _repo.GetPlatformById(id);
            if(platformItem==null)
            {
                return NotFound();
            }
            return Ok(
                _mapper.Map<PlatformReadDto>(platformItem)
            );
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatformAsync(PlatformCreateDto platformCreateDto)
        {
            var platformModel = _mapper.Map<Platform>(platformCreateDto);
            
            _repo.CreatePlatform(platformModel);
            _repo.SaveChanges();

            var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);
            
            try
            {
                 await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch (System.Exception ex)
            {                
                Console.WriteLine($"--> cound not send asynchronously: {ex.Message}");
            }

            return CreatedAtRoute(nameof(GetPlatformById), new {Id = platformReadDto.Id}, platformReadDto);
        }
    }
}