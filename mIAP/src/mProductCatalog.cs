using System;
using System.Collections.Generic;
using System.Net;
using mFramework.GameEvents;
using UnityEngine;
using UnityEngine.Purchasing;

namespace mFramework.IAP
{
    public struct RequestProductsState
    {
        public Action OnSuccess;
        public Action<Exception> OnFail;
    }

    public static class mProductCatalog
    {
        public static event Action Updated;

        public static string RemoteUrl = "";
        public static IEnumerable<CatalogItem> Items => _shopItems.Values;

        private static readonly Dictionary<string, CatalogItem> _shopItems;

        static mProductCatalog()
        {
            _shopItems = new Dictionary<string, CatalogItem>();
        }

        public static CatalogItem GetItem(string id)
        {
            if (_shopItems.ContainsKey(id))
                return _shopItems[id];
            return null;
        }

        public static void OnSuccessPurchase(Product product)
        {
            if (_shopItems.ContainsKey(product.definition.id) && !string.IsNullOrWhiteSpace(_shopItems[product.definition.id].Event))
            {
                mGameEvents.InvokeEvent(_shopItems[product.definition.id].Event, product);
            }
        }

        public static void GetProductsRemote(RequestProductsState state)
        {
            using (var webClient = new WebClient())
            {
                webClient.DownloadStringCompleted += WebClientOnDownloadStringCompleted;
                webClient.DownloadStringAsync(new Uri(RemoteUrl), state);
            }
        }

        private static void WebClientOnDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            var state = (RequestProductsState) e.UserState;
            if (e.Error != null || e.Cancelled)
            {
                state.OnFail?.Invoke(e.Error);
                return;
            }

            var json = SimpleJSON.JSON.Parse(e.Result);
            if (json == null || json["shopItems"] == null || !json["shopItems"].IsArray)
                return;

            foreach (var property in json["shopItems"].Children)
            {
                if (property["id"] == null)
                    continue;

                var id = property["id"].Value?.Trim();
                var enabled = property["enabled"]?.AsBool ?? true;
                var discountText = property["discount_text"]?.Value ?? string.Empty;
                var @event = property ["event"]?.Value ?? string.Empty;

                if (string.IsNullOrWhiteSpace(id))
                    continue;
                
                if (property["productType"] == null ||
                    !Enum.TryParse(property["productType"].Value, true, out ProductType type))
                {
                    continue;
                }

                IDs ids = null;
                if (property["storesIds"] != null && 
                    property["storesIds"].IsArray && 
                    property["storesIds"].Count > 0)
                {
                    ids = new IDs();
                    foreach (var store in property["storesIds"].Children)
                    {
                        if (store["store"] == null || store["id"] == null)
                            continue;
                        ids.Add(store["id"].Value, store["store"].Value);
                    }
                }

                if (_shopItems.ContainsKey(id))
                {
                    _shopItems[id].Type = type;
                    _shopItems[id].StoreIDs = ids;
                    _shopItems[id].Payouts = null;
                    _shopItems[id].Enabled = enabled;
                    _shopItems[id].DiscountText = discountText;
                    _shopItems[id].Event = @event;
                }
                else
                {
                    AddProduct(new CatalogItem(id, type, ids, null, enabled, @event)
                    {
                        DiscountText = discountText
                    });
                }
            }

            state.OnSuccess?.Invoke();
            Updated?.Invoke();
        }

        public static void AddProducts(IEnumerable<CatalogItem> items)
        {
            foreach (var shopItem in items)
            {
                AddProduct(shopItem);
            }
        }

        public static void AddProduct(CatalogItem item)
        {
            Debug.Log($"[mProductCatalog] Add product: {item}");

            if (_shopItems.ContainsKey(item.Id))
            {
                _shopItems[item.Id] = item;
                return;
            }

            _shopItems.Add(item.Id, item);
        } 
    }
}