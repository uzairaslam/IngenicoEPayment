using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ingenico.Connect.Sdk;
using Ingenico.Connect.Sdk.Domain.Definitions;
using Ingenico.Connect.Sdk.Domain.Payment;
using Ingenico.Connect.Sdk.Domain.Payment.Definitions;

namespace IngenicoEPayment
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                string paymentProductId = string.Empty;
                do
                {
                    Console.WriteLine("Enter Payment Product Code");
                    Console.WriteLine("Code | Card Type");
                    Console.WriteLine("1    | Visa");
                    Console.WriteLine("2    | Master Card");
                    paymentProductId = Console.ReadLine();
                } while (string.IsNullOrWhiteSpace(paymentProductId) || !new string[] { "1", "2" }.ToList().Contains(paymentProductId));
            

                Card card = new Card();
                card.CardNumber = "4567350000427977";
                card.Cvv = "123";
                card.ExpiryDate = "1220";

                ThreeDSecure threeDSecure = new ThreeDSecure();
                threeDSecure.SkipAuthentication = false;

                CardPaymentMethodSpecificInput cardPaymentMethodSpecificInput = new CardPaymentMethodSpecificInput();
                cardPaymentMethodSpecificInput.Card = card;
                cardPaymentMethodSpecificInput.PaymentProductId = paymentProductId == "2" ? 3 : 1;
                cardPaymentMethodSpecificInput.ThreeDSecure = threeDSecure;

                AmountOfMoney amountOfMoney = new AmountOfMoney();
                amountOfMoney.Amount = 1;
                amountOfMoney.CurrencyCode = "USD";

                Address billingAddress = new Address();
                billingAddress.CountryCode = "US";

                Customer customer = new Customer();
                customer.BillingAddress = billingAddress;
                customer.MerchantCustomerId = "1378";

                Order order = new Order();
                order.AmountOfMoney = amountOfMoney;
                order.Customer = customer;

                CreatePaymentRequest body = new CreatePaymentRequest();
                body.CardPaymentMethodSpecificInput = cardPaymentMethodSpecificInput;
                body.Order = order;

                try
                {
                    using (Client client = GetClient())
                    {
                        CreatePaymentResponse response = await client.Merchant("1378").Payments().Create(body);
                        Console.WriteLine("Id: " +response.Payment.Id);
                        Console.WriteLine("Status: " + response.Payment.Status);
                        Console.WriteLine("AmountOfMoney: " + response.Payment.PaymentOutput.AmountOfMoney);
                    }
                }
                catch (DeclinedPaymentException e)
                {
                    Console.WriteLine("Error: Payment Declined");
                    foreach (var error in e.Errors)
                    {
                        Console.WriteLine("Code: " + error.Code);
                        Console.WriteLine("PropertyName: " + error.PropertyName);
                        Console.WriteLine("Message: " + error.Message);
                    }
                }
                catch (ApiException e)
                {
                    Console.WriteLine("Error: Unknown error occured");
                    foreach (var error in e.Errors)
                    {
                        Console.WriteLine("Code: " + error.Code);
                        Console.WriteLine("PropertyName: " + error.PropertyName);
                        Console.WriteLine("Message: " + error.Message);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }

        private static Client GetClient()
        {
            string apiKeyId = "fb3d5e6642e17cef";
            string secretApiKey = "BWBmQIKxnhcH+WpkLUlLA7e0GLzla+SNYzUidmnTfFI=";

            CommunicatorConfiguration configuration = Factory.CreateConfiguration(apiKeyId, secretApiKey);
            return Factory.CreateClient(configuration);
        }
    }


}
