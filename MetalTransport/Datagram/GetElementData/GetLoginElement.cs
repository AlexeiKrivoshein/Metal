using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace MetalTransport.Datagram
{
    /// <summary>
    /// Попытка входа
    /// </summary>
    [Serializable]
    public sealed class GetLoginElement
        : GetElementData
    {
        public string Password { get; set;  } = "";

        public GetLoginElement() { }

        public GetLoginElement(Guid Id, string hash)
            : base(Id)
        {
            Password = hash;
        }
    }
}