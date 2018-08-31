using System;
using System.Linq;


namespace Nop.Plugin.Api.Client
{
    class Program
    {
        static void Main(string[] args)
        {

            // TODO: Change to match your environment
            var url = "http://localhost:15536";
            var clientId = "ea0204cd-5bbd-49ae-9810-d24735581f43";
            var clientSecret = "80a14949-8549-4d60-9d3a-864473b7a93a";

            

            var client = new NopApiClient(url, clientId, clientSecret);

            Console.Write("Fetching Orders...");
            var orders = client.Orders.GetOrders();

            Console.WriteLine($"received {orders.Count} orders.");


            ///////////////////////////////////////////
            // Example of how to go through each order
            ///////////////////////////////////////////
             
            var start = 0;

            while (true)
            {
                orders = client.Orders.GetOrders(start, limit: 1);

                var order = orders.LastOrDefault();
                if (order == null)
                {
                    break;
                }

                start = int.Parse(order.Id);
            }

            // now at this point we should store the 'start' variable


            Console.WriteLine("Hit any key to continue...");
            Console.ReadKey();

        }
    }
}
