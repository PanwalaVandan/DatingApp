using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    // [Authorize]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController: ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDatingRepository _repo;
        public MessagesController(IDatingRepository repo, IMapper mapper)
        {
            this._repo = repo;
            this._mapper = mapper;

        }

        [HttpGet("{id}", Name= "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id){
            // checks the token for currently logged in user
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var  messageFromRepo = await _repo.GetMessage(id);

            if(messageFromRepo == null){
                return NotFound();
            }

            return Ok(messageFromRepo);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, 
        [FromQuery]MessageParams messageParams){
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            messageParams.UserId = userId;

            var messageFromRepo = await _repo.GetMessagesForUser(messageParams);

            var messages = _mapper.Map<IEnumerable<MessageToReturnDto>>(messageFromRepo);

            Response.AddPagination(messageFromRepo.CurrentPage, 
            messageFromRepo.PageSize, 
            messageFromRepo.TotalCount, 
            messageFromRepo.TotalPages);

            return Ok(messages);
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId) {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var messageFromRepo = await _repo.GetMessageThread(userId, recipientId);
            var messageThread = _mapper.Map<IEnumerable<MessageToReturnDto>>(messageFromRepo);
            return Ok(messageThread);
        }


        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreationDto)
        {
            var sender = await _repo.GetUser(userId);
            // checks the token for currently logged in user
            if(sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            messageForCreationDto.SenderId = userId;

            var recipient = await _repo.GetUser(messageForCreationDto.RecipientId);
            if(recipient == null) {
                return BadRequest("Could not find user");
            }

            //Map dto into our message class
            var message = _mapper.Map<Message>(messageForCreationDto);

            _repo.Add(message);

            

            if(await _repo.SaveAll()){
            //this line maps the message back to messageforcreationdto as it is supposed to be return.
            //we use reverseMap() in the AutoMapperProfiles class to do this.
            var messageToReturn = _mapper.Map<MessageToReturnDto>(message);
            //-----reason to use CreatedatRoute here-------------
            //The correct response from an API for a successful post is to include the location 
            //in the header of where we can locate the newly created resource.  
            //In this case we return a 201 CreatedAtRoute with the location of the newly created resource.
            return CreatedAtRoute("GetMessage", new { id = message.Id}, messageToReturn);
            //return messageToReturn;
            }
            throw new Exception("Creating message failed on save");
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int id, int userId) {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var messageFromRepo = await _repo.GetMessage(id);

            if(messageFromRepo.SenderId == userId) {
                messageFromRepo.SenderDeleted = true;
            }

            if(messageFromRepo.RecipientId == userId) {
                messageFromRepo.RecipientDeleted = true;
            }

            // delete the message only if both side of conversation are deleting the message
            if(messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted){
                _repo.Delete(messageFromRepo);
            }

            if(await _repo.SaveAll()) {
                return NoContent();
            }

            throw new Exception("Error Deleting Message");
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkMessageAsRead(int userId, int id)
        {
             if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
             return Unauthorized();

             var message = await _repo.GetMessage(id);

             if(message.RecipientId != userId){
                 return Unauthorized();
             }

             message.isRead = true;
             message.DateRead = DateTime.Now;

             await _repo.SaveAll();

             return NoContent();


        }
    }
}