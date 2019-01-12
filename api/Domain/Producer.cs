using System.Collections.Generic;

namespace CoreJsNoise.Domain
{
    public class Producer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string FeedUrl { get; set; }

        public virtual ICollection<Show> Shows { get; set; }

        public Producer()
        {
            Shows = new List<Show>();
        }
    }
}