//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MetalDAL.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class LimitCardOperation
    {
        public System.Guid Id { get; set; }
        public int Index { get; set; }
        public System.Guid OrderId { get; set; }
        public System.Guid OperationId { get; set; }
        public short ElapsedHours { get; set; }
        public short ElapsedMinutes { get; set; }
        public double PricePerHour { get; set; }
    
        public virtual Order Order { get; set; }
        public virtual Operation Operation { get; set; }
    }
}
