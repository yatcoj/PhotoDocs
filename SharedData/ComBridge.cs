using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData
{
    public static class ComBridge
    {
        //A standard msg from one client to the server could look like this
        //draw,

        //A standard connect msg from a client could look like this
        //join,id,password?

        //To create and host a doc they would do this
        //make,username,width,height

        public static char SeperatorKey = ',';

        //Used to seperate multiple seperate draw commands that need to be sent to a single client
        //this way a new client wont need to be spammed with millions of seperate messages when they connect to an existing doc
        public static char MassMessageSeperatorKey = ';';

        public static string DrawKeyword = "draw";

        public static string HostDocKeyword = "make";

        public static string JoinDocKeyword = "join";

        public static char EndOfMsgKey = '!';

        //We need to define a key that will prevent pixels from being drawn on the canvas
        public static uint DontDrawKey = 0;

        //the location of the mouse in the wpf form is way off so this should help
        public static int MouseOffsetX = 0;

        public static int MouseOffsetY = 0;

    }
}
