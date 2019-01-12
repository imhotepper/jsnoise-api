using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreJsNoise.Domain;
using CoreJsNoise.Dto;
using CoreJsNoise.Handlers;
using CoreJsNoise.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Remotion.Linq.Clauses;

namespace CoreJsNoise.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public partial class ProducersController : ControllerBase
    {
        private IMediator _mediator;

        public ProducersController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        [Route("/api/admin/producers")]
        [Authorize]
        public async Task<ActionResult<Producer>> Post([FromBody] Producer producer)
        {
            if (producer == null || !ModelState.IsValid) return BadRequest(ModelState);

            return await _mediator.Send(new ProducerPostRequest {Producer = producer});
        }

        [HttpGet]
        [Route("update")]
        public async Task<ActionResult> Update()
        {
            return Ok(await _mediator.Send(new ShowsUpdateRequest()));
        }

        [HttpGet] 
        [Route("/api/producers/{id}/shows")]
        public async Task<ActionResult<ShowsResponse>> GetAll(int id, string q, int? page = 1) =>
        await _mediator.Send(new ProducerGetAllRequest {ProducerId = id, Query = q, Page = page});
        

        [HttpGet]
        [Route("/api/admin/producers")]
        [Authorize]
        public async Task<ActionResult<List<ProducerAggregateDto>>> GetProducers() =>
             await _mediator.Send(new ProducersForAdminRequest());
        
    }
}