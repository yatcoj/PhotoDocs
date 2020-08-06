using SharedData;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace PhotoDocs
{
    public class Tcp
    {
        private string ip = "127.0.0.1";
        private int port = 7117;
        private ASCIIEncoding asen = new ASCIIEncoding();
        private Stream stm;
        private TcpClient tcpclnt;

        private PhotoDocsManager pdw;
        private System.Threading.Thread workerThread;

        public Tcp(string ip, int port, PhotoDocsManager pdw)
        {
            this.port = port;
            this.pdw = pdw;

            this.ip = ConfigurationManager.AppSettings["ServerIp"];
            this.port = int.Parse(ConfigurationManager.AppSettings["ServerPort"]);
        }

        public void TcpClient()
        {
            try
            {
                this.tcpclnt = new TcpClient();
                Console.WriteLine("Connecting.....");
                tcpclnt.Connect(IPAddress.Parse(ip), port);

                Console.WriteLine("Connected");

                this.stm = tcpclnt.GetStream();
                System.Threading.Thread listenerThread = new System.Threading.Thread(new System.Threading.ThreadStart(TcpListener));
                listenerThread.Start();
                workerThread = listenerThread;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }
        }

        public void UpdateIp(string ip)
        {
            this.ip = ip;
        }

        public void SendMsg(string msg)
        {
            byte[] ba = asen.GetBytes(msg);
            stm.Write(ba, 0, ba.Length);
        }

        public void SendObject(ShapeData msg)
        {
            try
            {
                var formatter = new BinaryFormatter();

                formatter.Serialize(stm, msg);
            }
            catch (Exception e) { }
        }

        private void TcpListener()
        {
            bool listening = true;
            while (listening)
            {
                /*
                byte[] bb = new byte[7000];
                int k = stm.Read(bb, 0, 7000);

                string responceStr = "";
                for (int i = 0; i < k; i++)
                {
                    responceStr += Convert.ToChar(bb[i]);
                }
                pdw.RecevedMsg(responceStr);*/

                try
                {
                    ShapeData sd;
                    BinaryFormatter bf = new BinaryFormatter();
                    sd = (ShapeData)bf.Deserialize(stm);
                    pdw.RecevedMsg(sd);
                }
                catch(Exception e)
                {
                    stm.Close();
                    this.Close();
                }
            }

            tcpclnt.Close();
        }

        public void Close()
        {
            workerThread.Abort();
            this.stm.Close();
        }
    }
}
