using MetalTransport.Datagram;
using MetalTransport.Datagram.GetListData;
using MetalTransport.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetalTransport.ModelEx
{
    [Serializable]
    public class GetPlanElementsList
         : BaseDTO
    {
        public int Month { get; set; }
        public int Year { get; set; }

        public GetPlanElementsList()
        {
            Month = DateTimeHelper.DateTimeNow().Month;
            Year = DateTimeHelper.DateTimeNow().Year;
        }

        public GetPlanElementsList(int month, int year)
        {
            Month = month;
            Year = year;
        }
    }
}
