using GraphQL;
using GraphQL.Types;

namespace CoreJsNoise.GraphQL
{
    public class JsNoiseSchema: Schema
    {
        public JsNoiseSchema(IDependencyResolver resolver)
        {
            Query = resolver.Resolve<JsNoiseQuery>();
            Mutation = resolver.Resolve<JsNoiseMutation>();
        }   
    }
}