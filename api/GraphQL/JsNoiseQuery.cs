using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using CoreJsNoise.Domain;
using CoreJsNoise.Dto;
using CoreJsNoise.GraphQL.Types;
using CoreJsNoise.Handlers;
using GraphQL.Types;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreJsNoise.GraphQL
{
    public class JsNoiseQuery: ObjectGraphType
    { 
        public JsNoiseQuery( IMediator mediator)
        {
            Field<ShowsResponseType>(
                "showsList",
                arguments: new QueryArguments(
                    new QueryArgument<StringGraphType>{Name="q"},
                    new QueryArgument<IntGraphType>{Name="page"},
                    new QueryArgument<IntGraphType>{Name="producerId"}
                ),
                resolve: context => {
                      var producerId = context.GetArgument<int?>("producerId");  
                      var q = context.GetArgument<string>("q");  
                      var page = context.GetArgument<int?>("page");
                      page = !page.HasValue ? 1 : page;
                    return  !producerId.HasValue ?
                             mediator.Send(new ShowsRequest {Page = page, Query = q}):
                             mediator.Send(new ProducerGetAllRequest {ProducerId = producerId.Value, Query = q, Page = page});
                });

            Field<ListGraphType<ProducerAggregateDtoType>>(
                "producers",
                resolve : context => {
                    var user = (ClaimsPrincipal) context.UserContext;
                    if (user == null) return new  List<ProducerAggregateDto>();
                    return mediator.Send(new ProducersForAdminRequest());
                    }
            );

            Field<ShowDtoType>(
                name: "show",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType< IntGraphType>>{Name="id"}),
                resolve: context =>
                {
                    var sId = context.GetArgument<int>("id");
                    return mediator.Send(new ShowRequest {Id = sId});
                }
            );
        }
    } 
}