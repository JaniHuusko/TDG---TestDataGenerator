using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericTestDataCreator.Models
{
    public class Permutation
    {
        public int Count { get; set; }
        public List<PermutationConfiguration> Configuration { get; set; } = new List<PermutationConfiguration>();
    }
}
