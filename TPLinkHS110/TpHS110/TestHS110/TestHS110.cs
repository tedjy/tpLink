using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TpHS110;

namespace TestHS110
{
    internal class TestHS110
    {
        public TestHS110()
        {


            try
            {
                Console.WriteLine("saisir une addresse ip : ");
                //string ip = Console.ReadLine();
                HS110 prise = new HS110("172.16.69.81");
               
                
                Console.WriteLine("Addesse mac :"+ prise.getMacAddress());
                Console.WriteLine("realy on: ");
             
                  prise.relayOn();
                Console.ReadKey();
                Console.WriteLine("realy off: ");
                prise.relayOff();
                Console.ReadKey();
                Hs110Mesure h1 = prise.getMesure();
                Console.WriteLine("le courant en Ampères vaut " + h1.current);
                Console.ReadKey();
                Console.WriteLine("le voltage en Volt vaut " + h1.voltage);
                Console.ReadKey();
                Console.WriteLine("la puissance en Watt vaut " + h1.power);
                Console.ReadKey();
                Console.WriteLine("le tatal en puissance consomée vaut " + h1.total);
                Console.ReadKey();
                Console.WriteLine("err-code " + h1.err_code);
                Console.ReadKey();

            }
            catch (Exception)
            {

                throw;
            }
          

        }

        static void Main(string[] args)
        {
            new TestHS110();
        }
    }
}
