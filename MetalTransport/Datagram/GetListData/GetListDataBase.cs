using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetalTransport.Datagram.GetListData
{
    [Serializable]
    public abstract class GetListDataBase
        : BaseDTO
    {
        public bool ExcludeDelete = false;

        protected GetListDataBase() { }

        public GetListDataBase(bool excludeDelete)
        {
            ExcludeDelete = excludeDelete;
        }
    }
}
