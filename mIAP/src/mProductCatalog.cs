using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine.Purchasing;

namespace mFramework.IAP
{
    public static class mProductCatalog
    {
        public static IEnumerable<CatalogItem> Items => _shopItems.Values;

        private static readonly Dictionary<string, CatalogItem> _shopItems;

        static mProductCatalog()
        {
            _shopItems = new Dictionary<string, CatalogItem>();
        }

        public static void OnSuccessPurchase(Product product)
        {
            if (_shopItems.ContainsKey(product.definition.id))
                _shopItems[product.definition.id].OnPurchase?.Invoke(product);
        }

        public static void GetProductsRemote(string remoteUrl, Action onSuccess, Action onFail)
        {
            using (var webClient = new WebClient())
            {
                webClient.DownloadStringAsync(new Uri(remoteUrl), new [] {onSuccess, onFail});
                webClient.DownloadStringCompleted += WebClientOnDownloadStringCompleted;
            }
        }

        private static void WebClientOnDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                var onFail = ((Action[]) e.UserState)[1];
                onFail?.Invoke();
                return;
            }

            /*
                {
                    "shopItems": [
                        {
                            "id": "viewers.1000",
                            "enabled": "false",
                            "productType": "Consumable",
                            "storesIds": [
                                {
                                    "store": "GooglePlay",
                                    "id": "googleplay.viewers.1000"
                                },
                                {
                                    "store": "AppleAppStore",
                                    "id": "appstore.viewers.1000"
                                }
                            ]
                        },
                        {
                            "id": "viewers.9999",
                            "enabled": "true",
                            "productType": "Consumable",
                            "storesIds": [
                                {
                                    "store": "GooglePlay",
                                    "id": "googleplay.viewers.9999"
                                },
                                {
                                    "store": "AppleAppStore",
                                    "id": "appstore.viewers.9999"
                                }
                            ]
                        }
                    ]
                }
            */

            var json = SimpleJSON.JSON.Parse(e.Result);
            if (json == null || json["shopItems"] == null || !json["shopItems"].IsArray)
                return;

            foreach (var property in json["shopItems"].Children)
            {
                if (property["id"] == null)
                    continue;

                var id = property["id"].Value;
                var enabled = property["enabled"]?.AsBool ?? true;

                if (string.IsNullOrWhiteSpace(id) || !_shopItems.ContainsKey(id))
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

                _shopItems[id].Type = type;
                _shopItems[id].StoreIDs = ids;
                _shopItems[id].Payouts = null;
                _shopItems[id].Enabled = enabled;
            }

            var onSuccess = ((Action[]) e.UserState)[0];
            onSuccess?.Invoke();
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
            mCore.Log($"[mProductCatalog] Add product: {item}");
            if (_shopItems.ContainsKey(item.Id))
            {
                _shopItems[item.Id] = item;
                return;
            }

            _shopItems.Add(item.Id, item);
        } 
    }
}