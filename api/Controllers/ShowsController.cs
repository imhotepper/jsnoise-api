using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreJsNoise.Domain;
using CoreJsNoise.Dto;
using CoreJsNoise.Handlers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreJsNoise.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class ShowsController : ControllerBase
    {
        private IMediator _mediator;
        
        public ShowsController(PodcastsCtx db, IMediator mediator) => _mediator = mediator;

        [HttpGet]
        [Route("/api/showslist")]
        public async Task<ActionResult<ShowsResponse>> GetAll(string q, int? page = 1) 
            => await _mediator.Send(new ShowsRequest {Page = page, Query = q});

        [HttpGet]
        [Route("/api/shows/{id}")]
        public async Task<ActionResult >Get(int id)
        {      
            var resp = await _mediator.Send(new ShowRequest {Id = id});                     
            if (resp == null)  return NotFound();
            return Ok(resp);           
        }
    }
}