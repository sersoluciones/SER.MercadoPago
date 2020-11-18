using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using MercadoPago.Common;

namespace MercadoPago.DataStructures.MerchantOrder
{
    public struct MerchantOrderPayment
    {
        #region Properties


        public enum OperationType
        {
            RegularPayment,
            PaymentAddition
        }

        #endregion

        #region Accessors

        public string ID { get; }

        public float TransactionAmount { get; }

        public float TotalPaidAmount { get; }

        public float ShippingCost { get; }

        public CurrencyId PaymentCurrencyId
        {
            get;        
        }

        public string Status { get; }

        public string StatusDetail { get; }

        public OperationType PaymentOperationType { get; }

        public DateTime DateApproved { get; }

        public DateTime DateCreated { get; }

        public DateTime LastModified { get; }

        public float AmountRefunded { get; }

        #endregion
    }
}
