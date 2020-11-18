using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MercadoPago.DataStructures.Customer
{
    public struct Shipment
    {
        #region Accessors

        public bool Success { get; }
        public List<Error> Errors { get; }

        public string Name { get; }

        #endregion
    }
}
