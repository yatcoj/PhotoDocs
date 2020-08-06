using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PhotoDocsServer
{
    public class GroupManager
    {
        private Dictionary<int, Group> groups= new Dictionary<int, Group>();

        public void Update()
        {

        }

        public Group MakeGroup(Socket s, int width, int height)
        {
            Group g = new Group(s, width, height);
            groups.Add(groups.Count, g);
            return g;
        }

        public Group JoinGroup(int groupId, Socket s)
        {
            if(groups.ContainsKey(groupId))
            {
                groups[groupId].AddMember(s);
                return groups[groupId];
            }
            return null;
        }
    }
}
