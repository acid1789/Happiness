using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Braintree;

namespace GlobalServer
{
    class BraintreeManager
    {
        static BraintreeGateway _gateway;
        static BraintreeGateway Gateway
        {
            get
            {
                if (_gateway == null)
                {
                    _gateway = new BraintreeGateway
                    {
                        Environment = Braintree.Environment.SANDBOX,
                        MerchantId = "5q8xpz6xx3w2zg7g",
                        PublicKey = "z8x57m8bhvr65hn6",
                        PrivateKey = "5b07c07b8679513ee252f488d3163329"
                    };
                }
                return _gateway;
            }
        }

        public static string GetClientToken(string customerId)
        {
            string clientToken = Gateway.ClientToken.generate();//new ClientTokenRequest { CustomerId = customerId });
            return clientToken;
        }

        public static string DoPurchase(string nonce, string uid, string pid)
        {
            NetworkCore.GlobalProduct p = Marketplace.Instance.GetProduct(pid);
            var request = new TransactionRequest
            {
                Amount = (decimal)p.USD,
                PaymentMethodNonce = nonce,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            Result<Transaction> result = Gateway.Transaction.Sale(request);
            if (result.IsSuccess())
            {
                string transactionJson = Newtonsoft.Json.JsonConvert.SerializeObject(result.Target);
                Marketplace.Instance.FinalizeProductPurchase(uid, pid, transactionJson);
                return transactionJson;
            }
            
             
            return result.Message;
        }
        
        
    }
}
