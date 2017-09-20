using inRiver.Remoting;
using inRiver.Remoting.Exceptions;
using inRiver.Remoting.Objects;
using inRiver.Remoting.Security;
using System;
using System.Security.Authentication;

namespace TestInRiver
{
    class Program
    {
        static void Main(string[] args)
        {
            Init();

            var prod = RemoteManager.DataService.GetAllEntitiesForEntityType("Product", LoadLevel.Shallow);


            //var modelSrvc = new PIM_ModelService();
            //modelSrvc.AddProductEntity();

            var dataSrvc = new PIM_DataService();
            dataSrvc.GetChannel();

            var userSrvc = new PIM_UserService();
            userSrvc.Settings();

            Console.WriteLine("Authentication Succeeded. Now you can start using the inRiver Remoting API");

            Console.ReadLine();
        }

        static void Init()
        {
            AuthenticationTicket ticket;
            try
            {
                ticket = RemoteManager.Authenticate("http://dev.inriver.com:8080", "pimuser1", "pimuser1");
            }
            //catch (EndpointNotFoundException epEx)
            //{
            //    Console.WriteLine("Authentication failed for communication reason!");
            //    Console.WriteLine(string.Format("Error message: {0}", epEx.Message));
            //    Console.ReadLine();
            //    return;
            //}
            catch (SecurityException secEx)
            {
                Console.WriteLine("Authentication failed for Security reason!");
                Console.WriteLine(string.Format("Error message: {0}", secEx.Message));
                Console.ReadLine();
                return;
            }
            catch (AuthenticationException autEx)
            {
                Console.WriteLine("Authentication failed!");
                Console.WriteLine(string.Format("Error message: {0}", autEx.Message));
                Console.ReadLine();
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Authentication failed for unknown reason!");
                Console.WriteLine(string.Format("Error message: {0}", ex.Message));
                Console.ReadLine();
                return;
            }

            if (ticket == null)
            {
                Console.WriteLine("Authentication failed! No ticket created.");
                Console.ReadLine();
                return;
            }

            RemoteManager.CreateInstance("http://dev.inriver.com:8080", ticket);
        }
    }
}
