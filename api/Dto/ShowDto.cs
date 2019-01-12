using System;

namespace CoreJsNoise.Dto
{
    public class ShowDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Mp3 { get; set; }
        public DateTime PublishedDate { get; set; }
        public string ProducerName { get; set; }
        public int ProducerId { get; set; }
    }
}  