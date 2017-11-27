using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Purchasing;

namespace mFramework.IAP
{
    public class CatalogItem
    {
        internal Action<Product> OnPurchase { get; }

        public string Id { get; internal set; }
        public ProductType Type { get; internal set; }
        public IDs StoreIDs { get; internal set; }
        public IEnumerable<PayoutDefinition> Payouts { get; internal set; }
        public bool Enabled { get; internal set; }
        
        public CatalogItem(string id, ProductType type, IDs storeIDs, IEnumerable<PayoutDefinition> payouts, 
            bool enabled = true, Action<Product> onPurchase = null)
        {
            Id = id;
            Type = type;
            StoreIDs = storeIDs;
            Payouts = payouts;
            Enabled = enabled;
            OnPurchase = onPurchase;
        }

        public override string ToString()
        {
            return $"(id={Id} enabled={Enabled} type={Type} payouts={Payouts} ids={StoreIDs?.Select(s => $"{s.Key}={s.Value}").Aggregate((i1, i2) => $"{i1}, {i2}")})";
        }
    }
}
