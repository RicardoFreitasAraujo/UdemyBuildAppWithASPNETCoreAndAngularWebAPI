using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.Api.Data;
using DatingApp.Api.Dtos;
using DatingApp.Api.Helpers;
using DatingApp.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Api.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository repo;
        private readonly IMapper mapper;

        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            this.repo = repo;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var users = await this.repo.GetUsers(userParams);
            var usersToReturn = this.mapper.Map< IEnumerable<User>,IEnumerable <UserForListDto>>(users);

            Response.AddPagination(users.CurrentPage, users.PageSize, 
                                   users.TotalCount, users.TotalPages);

            return Ok(usersToReturn);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await this.repo.GetUser(id);

            var userToReturn = this.mapper.Map<User,UserForDetailDto>(user);

            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
        {

            int idUserLogged = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (id != idUserLogged)
                return Unauthorized();

            User userFromRepo = await this.repo.GetUser(id);

            this.mapper.Map(userForUpdateDto, userFromRepo);

            if (await this.repo.SaveAll())
                return NoContent();

            throw new Exception($"Updating user {id} failed on saved");

        }

        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
            int idUserLogged = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (id != idUserLogged)
                return Unauthorized();

            var like = await this.repo.GetLike(id, recipientId);

            if (like != null)
                return BadRequest("You already like this user");

            if (await this.repo.GetUser(recipientId) == null)
                return NotFound();

            Like likeAdd = new Like
            {
                LikerId = id,
                LikeeId = recipientId
            };

            this.repo.Add<Like>(likeAdd);
            if (await this.repo.SaveAll())
                Ok();

            return BadRequest("Failed to like user");
        }

    }
}