using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkCore;
using ServerCore;

namespace GlobalServer
{
    class Marketplace
    {
        static Marketplace _instance;
        public static Marketplace Instance { get { return _instance; } }

        GlobalProduct[] _products;
        ServerBase _server;

        public Marketplace(ServerBase server)
        {
            _instance = this;
            _server = server;
        }

        public void SetProducts(GlobalProduct[] products)
        {
            // Save our copy here
            _products = products;

            // Send to all connected game servers
            if (_products != null)
            {
                Connection[] gameServers = _server.InputThread.Clients;
                foreach (Connection c in gameServers)
                {
                    GlobalClient gc = (GlobalClient)c;
                    if (gc != null)
                        gc.SendProducts(_products);
                }
            }
        }

        public GlobalProduct GetProduct(string pid)
        {
            int id = -1;
            if (int.TryParse(pid, out id))
            {
                foreach (GlobalProduct p in _products)
                {
                    if (p.ProductId == id)
                        return p;
                }
            }
            return null;
        }

        public void FinalizeProductPurchase(string uid, string pid, string transactionJson)
        {
            int user = -1;            
            if (int.TryParse(uid, out user))
            {
                GlobalProduct p = GetProduct(pid);
                
                GlobalTask gt = new GlobalTask(GlobalTask.GlobalType.Purchase_Product);
                gt.Args = new object[] { user, p.ProductId, p.Coins, p.VIP, p.USD, 0 /*braintree*/, transactionJson.Replace('\"','\'') };

                _server.TaskProcessor.AddTask(gt);
            }
        }

        #region Accessors
        public GlobalProduct[] Products { get { return _products; } }
        #endregion
    }
}
