using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFramework
{
    public class AllotmentSummary
    {
        public int RoundNo { get; set; }
        public string Instcd { get; set; }
        public string Brcd { get; set; }
        public string Sequence { get; set; }
        public int Seats { get; set; }
        public int Allotted { get; set; }
        public Double OpeningRank { get; set; }
        public Double ClosingRank { get; set; }
        public int DereserveFrom { get; set; }
        public int DereserveTo { get; set; }
    }
}
