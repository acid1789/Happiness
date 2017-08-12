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

        public static string UserIDToCustomerId(string userId)
        {
            string btCustomer = "rgc_" + userId;
            return btCustomer;
        }

        public static string GetClientToken(string customerId)
        {
            string btCustomer = UserIDToCustomerId(customerId);
            Customer c = null;
            try { c = Gateway.Customer.Find(btCustomer); } catch (Exception) { }
            string clientToken = (c == null) ? Gateway.ClientToken.generate() : Gateway.ClientToken.generate(new ClientTokenRequest { CustomerId = btCustomer });
            return clientToken;
        }

        public static string DoPurchase(string nonce, string uid, string pid)
        {
            string btCustomer = UserIDToCustomerId(uid);
            Customer c = null;
            try { c = Gateway.Customer.Find(btCustomer); } catch (Exception) { }
            /*
            try
            {
                if (c == null)
                {
                    Result<Customer> cr = Gateway.Customer.Create(new CustomerRequest() { CustomerId = btCustomer, PaymentMethodNonce = nonce });
                    ServerCore.LogThread.GetLog().Log(NetworkCore.LogInterface.LogMessageType.System, true, "Braintree create customer: " + cr.Message);
                }
                else
                {
                    Result<Customer> cr = Gateway.Customer.Update(btCustomer, new CustomerRequest() { CustomerId = btCustomer, PaymentMethodNonce = nonce });
                    ServerCore.LogThread.GetLog().Log(NetworkCore.LogInterface.LogMessageType.System, true, "Braintree update customer: " + cr.Message);
                }
            }
            catch (Exception) { }
            */

            NetworkCore.GlobalProduct p = Marketplace.Instance.GetProduct(pid);
            var request = new TransactionRequest
            {
                Amount = (decimal)p.USD,
                PaymentMethodNonce = nonce,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true,
                    StoreInVaultOnSuccess = true
                }
            };
            if (c == null)
                request.Customer = new CustomerRequest() { Id = btCustomer };
            else
                request.CustomerId = btCustomer;

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
