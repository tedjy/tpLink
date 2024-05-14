using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TestHS110;

namespace TpHS110
{
    internal class HS110
    {
        private string ipAddress;
        TcpClient tcpclnt;
        public HS110(string ipAddress)
        {
            this.ipAddress = ipAddress;


        }

   
        public void relayOn()
        {
            TcpClient tcpclnt = new TcpClient();
        
            tcpclnt.Connect(ipAddress, 9999);// adapter selon port et IP


            //envoie
            Stream stm = tcpclnt.GetStream(); // recupère stream pour read/ write
            byte[] ba = encrypt("{\"system\":{\"set_relay_state\":{\"state\":1}}}"); // envoyer message                                                                     
            stm.Write(ba, 0, ba.Length);
            stm.Flush();

            //reception reponse
            byte[] bb = new byte[1000];
            int b = stm.ReadByte();
            int k = stm.Read(bb, 0, 1000); // lire message
            string convert = decrypt(bb); // convertir byte[]  en string et afficher
            //Console.WriteLine(convert);

            //fermeture
            tcpclnt.Close();
        }
        public void relayOff()
        {
            TcpClient tcpclnt = new TcpClient();

            tcpclnt.Connect(ipAddress, 9999);// adapter selon port et IP


            //envoie
            Stream stm = tcpclnt.GetStream(); // recupère stream pour read/ write
            byte[] ba = encrypt("{\"system\":{\"set_relay_state\":{\"state\":0}}}"); // envoyer message                                                                     
            stm.Write(ba, 0, ba.Length);
            stm.Flush();

            //reception reponse
            byte[] bb = new byte[1000];
            int b = stm.ReadByte();
            int k = stm.Read(bb, 0, 1000); // lire message
            string convert = decrypt(bb); // convertir byte[]  en string et afficher
            //Console.WriteLine(convert);

            //fermeture
            tcpclnt.Close();
        }
        public string getMacAddress()
        {
            TcpClient tcpclnt = new TcpClient();

            tcpclnt.Connect(ipAddress, 9999);// adapter selon port et IP


            //envoie
            Stream stm = tcpclnt.GetStream(); // recupère stream pour read/ write
            byte[] ba = encrypt("{\"system\":{\"get_sysinfo\":{}}}"); // envoyer message                                                                     
            stm.Write(ba, 0, ba.Length);
            stm.Flush();

            //reception reponse
            byte[] bb = new byte[1000];
            int b = stm.ReadByte();
            int k = stm.Read(bb, 0, 1000); // lire message
            string reponse = decrypt(bb); // convertir byte[]  en string et afficher
            //Console.WriteLine(convert);
            int pos = reponse.IndexOf("mac");
            string mac = reponse.Substring(pos +6, 17);
            //fermeture
            tcpclnt.Close();
            return mac ;
        }

        public Hs110Mesure getMesure()
        {
            TcpClient tcpclnt = new TcpClient();

            tcpclnt.Connect(ipAddress, 9999);// adapter selon port et IP


            //envoie
            Stream stm = tcpclnt.GetStream(); // recupère stream pour read/ write
            byte[] ba = encrypt("{\"emeter\":{ \"get_realtime\":null } }"); // envoyer message                                                                     
            stm.Write(ba, 0, ba.Length);
            stm.Flush();

            //reception reponse
            byte[] bb = new byte[1000];
            int b = stm.ReadByte();
            int k = stm.Read(bb, 0, 1000); // lire message
            string reponse = decrypt(bb); // convertir byte[]  en string et afficher
            //Console.WriteLine(convert);
           
            //fermeture
            tcpclnt.Close();
            
            Hs110Mesure hs110 = new Hs110Mesure();
            //int pos = reponse.IndexOf();
            int posCurrent = reponse.IndexOf("current");
            string courrant = reponse.Substring(posCurrent + 9, 8);
            double cour = Convert.ToDouble(courrant,new CultureInfo("en-US"));
            hs110.current = cour;

            int posVoltage = reponse.IndexOf("voltage");
            string volt = reponse.Substring(posVoltage + 9, 10);
            hs110.voltage = Convert.ToDouble(volt, new CultureInfo("en-US"));

            int posPower = reponse.IndexOf("power");
            string pow = reponse.Substring(posPower + 7, 1);
            hs110.power = Convert.ToDouble(pow, new CultureInfo("en-US"));

            int posTotal = reponse.IndexOf("total");
            string total = reponse.Substring(posTotal + 7, 1);
            hs110.total = Convert.ToDouble(total, new CultureInfo("en-US"));

            int posErr = reponse.IndexOf("err_code");
            string err_code = reponse.Substring(posErr + 4, 1);
            hs110.err_code = err_code;
           
            return hs110;
        }
        private byte[] encrypt(String message)
        {

            byte[] data = Encoding.ASCII.GetBytes(message);

            byte[] enc = new byte[data.Length + 4];

            byte[] intBytes = BitConverter.GetBytes(data.Length);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(intBytes);
            intBytes.CopyTo(enc, 0);

            data.CopyTo(enc, 4);
            byte key = 0xAB; // KEY vaut 0xAB
            for (int i = 4; i < enc.Length; i++)
            {
                enc[i] = (byte)(enc[i] ^ key);
                key = enc[i];
            }
            return enc;
        }
        private String decrypt(byte[] data)
        {
            if (data == null)
            {
                return null;
            }
            byte key = 0xAB; // KEY vaut 0xAB
            byte nextKey = 0;
            for (int i = 4; i < data.Length; i++)
            {
                nextKey = data[i];
                data[i] = (byte)(data[i] ^ key);
                key = nextKey;
            }
            string s = Encoding.ASCII.GetString(data, 4, data.Length - 4);
            return s;
        }
        
    }


  
}
