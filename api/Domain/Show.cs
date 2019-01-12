using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreJsNoise.Domain
{
    public class Show
    {
        public int Id { get; set; }
        public int ProducerId { get; set; }
        public virtual Producer Producer { get; set; }
        public string Title { get; set; }
        [Column(TypeName = "text")]
        public string Description { get; set; }
        public string Mp3 { get; set; }
        public DateTime PublishedDate { get; set; }
    }
}