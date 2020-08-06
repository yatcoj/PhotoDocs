using SharedData;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PhotoDocsServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            Program pg = new Program();
            pg.Runner();
            
        }

        public GroupManager gm = new GroupManager();

        private void Runner()
        {
            try
            {
                string ip = ConfigurationManager.AppSettings["ServerIp"];
                string port = ConfigurationManager.AppSettings["ServerPort"];

                IPAddress ipAd = IPAddress.Parse(ip);

                TcpListener myList = new TcpListener(ipAd, int.Parse(port));
                myList.Start();
                Console.WriteLine("The server is running at port 8001...");
                Console.WriteLine("The local End point is  :" + myList.LocalEndpoint);
                Console.WriteLine("Waiting for a connection.....");

                bool online = true;
                Socket s = null;
                while (online)
                {
                    s = myList.AcceptSocket();
                    Console.WriteLine("Connection accepted from " + s.RemoteEndPoint);

                    Thread listenerThread = new Thread(() => WorkerThread(s));
                    listenerThread.Start();

                }
                s.Close();
                myList.Stop();

            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }
        }

        private void WorkerThread(Socket s)
        {
            Group myGroup = null;
            Thread incomingMsgManager = null;
            try
            {
                bool online = true;
                byte[] b = new byte[100];
                int k = s.Receive(b);
                string recevedMsg = "";
                for (int i = 0; i < k; i++)
                {
                    recevedMsg += Convert.ToChar(b[i]);
                }

                if (recevedMsg.Split(ComBridge.SeperatorKey)[0].Contains(ComBridge.HostDocKeyword))
                {
                    myGroup = gm.MakeGroup(s, int.Parse(recevedMsg.Split(ComBridge.SeperatorKey)[2]), int.Parse(recevedMsg.Split(ComBridge.SeperatorKey)[3]));
                }
                else
                {
                    try
                    {
                        myGroup = gm.JoinGroup(int.Parse(recevedMsg.Split(ComBridge.SeperatorKey)[2]), s);
                    }
                    catch
                    {
                        myGroup = gm.JoinGroup(0, s);
                    }
                }

                ConcurrentQueue<ShapeData> cq = new ConcurrentQueue<ShapeData>();
                cq.Enqueue(new ShapeData());
                incomingMsgManager = new Thread(() => IncomingMsgQueManager(cq, myGroup, s));
                incomingMsgManager.Start();
                cq.Enqueue(new ShapeData());

                while (online)
                {
                    ShapeData sd;
                    BinaryFormatter bf = new BinaryFormatter();
                    var myStream = new NetworkStream(s);
                    sd = (ShapeData)bf.Deserialize(myStream);
                    myStream.Close();

                    //myGroup.IncomingMessage(sd, s);
                    cq.Enqueue(sd);
                }
            }
            catch(Exception e)
            {
                if (myGroup != null)
                {
                    myGroup.RemoveMember(s);
                }
                s.Close();
            }
        }

        private void IncomingMsgQueManager(ConcurrentQueue<ShapeData> cq, Group myGroup, Socket mySocket)
        {
            bool online = true;
            while (online)
            {
                ShapeData sd = null;
                cq.TryDequeue(out sd);
                if (sd != null)
                {
                    lock (myGroup.incomingLock)
                    {
                        myGroup.IncomingMessage(sd, mySocket);
                    }
                }
            }
        }
    }
}
