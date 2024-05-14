using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TpHS110
{
    internal class HS110Test
    {
        public HS110Test()
        {
            try
            {
                //connexion
                TcpClient tcpclnt = new TcpClient();
                Console.WriteLine("Connecting.....");
                tcpclnt.Connect("172.16.69.81", 9999);// adapter selon port et IP
                Console.WriteLine("Connected");

                //envoie
                Stream stm = tcpclnt.GetStream(); // recupère stream pour read/ write
                ASCIIEncoding asen = new ASCIIEncoding();
                byte[] ba = encrypt("{\"system\":{\"get_sysinfo\":{}}}"); // envoyer message
                //byte[] ba = encrypt("{\"system\":{\"set_relay_state\":{\"state\":1}}}"); // envoyer message
                stm.Write(ba, 0, ba.Length);
                stm.Flush();

                //reception reponse
                byte[] bb = new byte[1000];
                int b = stm.ReadByte();
                int k = stm.Read(bb, 0, 1000); // lire message
                string convert = decrypt(bb); // convertir byte[]  en string et afficher
                Console.WriteLine(convert);

                //fermeture
                tcpclnt.Close();
            }

            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }

            Console.ReadKey();
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



        static void Main(string[] args)
        {
            new HS110Test();
        }
    }
}
