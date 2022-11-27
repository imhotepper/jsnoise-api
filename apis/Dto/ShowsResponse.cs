using System.Collections.Generic;

namespace CoreJsNoise.Dto
{
    public class ShowsResponse
    {
        public bool First { get; set; }
        public bool Last { get; set; }
        public List<ShowDto> Shows { get; set; }
    }
}