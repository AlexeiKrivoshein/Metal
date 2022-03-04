using MetalTransport.Datagram;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MetalServer.Handler
{
    [Handler]
    public sealed class FilterHandler
        : BaseHandler<FilterDTO, SetListData<BaseDTO>>
    {
        private List<DatagramType> _types = new List<DatagramType> { DatagramType.Filter };
        protected override List<DatagramType> Types() => _types;

        public override string ExceptionHeader() => "Не удалось выполнить отбор";

        protected override SetListData<BaseDTO> InnerHandle(FilterDTO data, DatagramType type)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            var elements = Manager.GetFiltered(data, out var count);

            return new SetListData<BaseDTO>(elements, count);
        }
    }
}
