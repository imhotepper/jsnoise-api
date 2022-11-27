namespace CoreJsNoise.Dto
{
    public partial class ProducersController
    {
        public class ProducerDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Url { get; set; }
            public string FeedUrl { get; set; }

        }
    }
}