

using GraphQL.Types;

public class ProducerInputType: InputObjectGraphType{
    public ProducerInputType()
    {
        Name = "producerInput";
        Field<NonNullGraphType<StringGraphType>>("name");
        Field<NonNullGraphType<StringGraphType>>("url");
        Field<NonNullGraphType<StringGraphType>>("FeedUrl");
    }
}