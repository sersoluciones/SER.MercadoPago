using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MercadoPago.DataStructures.Customer
{
    public struct Error
    {
        #region Accessors

        public string Code { get; }

        public string Description { get; }

        public string Field { get; }

        #endregion
    }
}
