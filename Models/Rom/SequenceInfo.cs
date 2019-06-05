using System.Collections.Generic;
using System.Linq;

namespace MMRando.Models.Rom
{

    public class SequenceInfo
    {
        public string Name { get; set; }
        public int Replaces { get; set; } = -1;
        public int MM_seq { get; set; } = -1;
        public List<int> Type { get; set; } = new List<int>();
        public int Instrument { get; set; }
        public bool CanReplace(SequenceInfo target)
        {
            return (Type.Intersect(target.Type).Count() > 0) || ((Type[0] & 8) == (target.Type[0] & 8)
                            && (Type.Contains(10) == target.Type.Contains(10))
                            && (!Type.Contains(16)));
        }
    }
}
