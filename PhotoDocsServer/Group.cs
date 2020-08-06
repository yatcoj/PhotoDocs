using SharedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace PhotoDocsServer
{
    public class Group
    {
        //Need list of all drawing changes
        //List of all members in group
        public Socket ownerSocket;
        public List<Socket> memberSockets = new List<Socket>();
        private ASCIIEncoding asen = new ASCIIEncoding();
        private List<ShapeData> drawItems = new List<ShapeData>();
        private ShapeData masterImage = new ShapeData();
        public readonly object incomingLock = new object();
        public readonly object outgoingLock = new object();

        //Methods to support updating the previous lists
        public Group(Socket s, int width, int height)
        {
            masterImage = new ShapeData(width, height);
            ownerSocket = s;
            memberSockets.Add(s);
        }

        public void AddMember(Socket s)
        {
            lock(incomingLock)
            {
                memberSockets.Add(s);
                //Send all drawing changes to new member here?
                UpdateClient(s);
            }
        }

        public void RemoveMember(Socket s)
        {
            lock (incomingLock)
            {
                memberSockets.Remove(s);
            }
        }

        public void IncomingMessage(string msg, Socket s)
        {
            if(msg.Split(ComBridge.SeperatorKey)[0].Contains(ComBridge.DrawKeyword))
            {
                //drawItems.Add(msg);
                //BroadcastMessage(msg,s);
            }
        }

        public void IncomingMessage(ShapeData msg, Socket s)
        {
            drawItems.Add(msg);
            masterImage.ApplyShape(msg);
            BroadcastMessage(msg, s);
            if(drawItems.Count > 2000)
            {
                drawItems.Clear();
            }
        }

        public void BroadcastMessage(ShapeData msg, Socket sender)
        {
            foreach (Socket s in memberSockets)
            {
                if (s != sender)
                {
                    var formatter = new BinaryFormatter();
                    var myStream = new NetworkStream(s);
                    formatter.Serialize(myStream, msg);
                }
            }
        }

        public void UpdateClient(Socket client)
        {
            //Need to update this so that i can send more than one "item" at once by using the 
            //ComBridge.MassMessageSeperatorKey
            /*
            string strWithSplit = ComBridge.MassMessageSeperatorKey+"";
            foreach (ShapeData str in drawItems)
            {
                strWithSplit += str;
            }
            client.Send(asen.GetBytes(strWithSplit + ComBridge.SeperatorKey + ComBridge.MassMessageSeperatorKey));*/
            var formatter = new BinaryFormatter();
            var myStream = new NetworkStream(client);
            formatter.Serialize(myStream, masterImage);
        }
    }
}
