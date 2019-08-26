
using CoreJsNoise.Domain;
using CoreJsNoise.Dto;
using CoreJsNoise.Handlers;
using GraphQL.Types;
using MediatR;

public class JsNoiseMutation : ObjectGraphType
{

    public JsNoiseMutation(IMediator mediator)
    {
        FieldAsync<ProducerType>(
            name: "createProducer",
            arguments: new QueryArguments(new QueryArgument<NonNullGraphType<ProducerInputType>> { Name = "producer" }),
            resolve: async context =>
            {
                var producer = context.GetArgument<Producer>("producer");
                return await context.TryAsyncResolve(
                    async c =>
                    {
                        var result = await mediator.Send(new ProducerPostRequest { Producer = producer });
                        return result;
                    });
            }
        );
    }
}