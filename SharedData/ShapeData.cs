using SharedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SharedData
{
    [Serializable()]
    public class ShapeData : ISerializable
    {
        //UpdateCanvas(uint[] pixels, int xOffset, int yOffset, int pixelWidth)
        public uint[] pixels;

        public int xOffset;

        public int yOffset;

        public int pixelWidth;

        public int pixelHeight;

        public ShapeData(uint[] pixels, int xOffset, int yOffset, int pixelWidth, int pixelHeight)
        {
            this.pixels = pixels;
            this.xOffset = xOffset;
            this.yOffset = yOffset;
            this.pixelWidth = pixelWidth;
            this.pixelHeight = pixelHeight;
        }

        public ShapeData(uint[] pixels, double xOffset, double yOffset, int pixelWidth, int pixelHeight)
        {
            this.pixels = pixels;
            this.xOffset = (int)xOffset;
            this.yOffset = (int)yOffset;
            this.pixelWidth = pixelWidth;
            this.pixelHeight = pixelHeight;
        }

        public ShapeData(string str)
        {
            if (str.StartsWith(ComBridge.DrawKeyword))
            {
                string[] strArr = str.Split(ComBridge.SeperatorKey);
                this.xOffset = int.Parse(strArr[1]);
                this.yOffset = int.Parse(strArr[2]);
                this.pixelWidth = int.Parse(strArr[3]);
                this.pixelHeight = int.Parse(strArr[4]);
                uint[] u = new uint[this.pixelWidth*this.pixelHeight];

                for(int i = 5; i < pixelWidth*pixelHeight+5; i++)
                {
                    u[i-5] = uint.Parse(strArr[i]);
                }
                this.pixels = u;
            }
        }

        public ShapeData(int width, int height)
        {
            this.pixelHeight = height;
            this.pixelWidth = width;
            pixels = new uint[width * height];
        }

        public ShapeData()
        {

        }

        public void ApplyShape(ShapeData shape)
        {
            int xOffset = shape.xOffset;
            int yOffset = shape.yOffset;
            int pixelWidth = shape.pixelWidth;
            uint[] newpixels = shape.pixels;

            int width = this.pixelWidth;
            int height = this.pixelHeight;

            //Gets the image from the canvas
            //bitmap = new WriteableBitmap((BitmapSource)(drawArea.Background as ImageBrush).ImageSource);

            for (int x = xOffset; x < xOffset + pixelWidth; ++x)
            {
                for (int y = yOffset; y < yOffset + pixelWidth; ++y)
                {
                    if (y >= 0 && y < height && x >= 0 && x < width)
                    {
                        int i = width * y + x;
                        int ip = pixelWidth * (y - yOffset) + (x - xOffset);
                        if (i < pixels.Length && ip < newpixels.Length)
                        {
                            if (newpixels[ip] != ComBridge.DontDrawKey)
                            {
                                pixels[i] = newpixels[ip];
                            }
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            //draw,xOffset,yOffset,pixelWidth,uint[]!;
            /*return $"{ComBridge.DrawKeyword}{ComBridge.SeperatorKey}" +
                $"{xOffset}{ComBridge.SeperatorKey}" +
                $"{yOffset}{ComBridge.SeperatorKey}" +
                $"{pixelWidth}{ComBridge.SeperatorKey}" +
                $"{pixelHeight}{ComBridge.SeperatorKey}" +
                $"{uintToString(pixels)}{ComBridge.SeperatorKey}{ComBridge.EndOfMsgKey}";*/
            return "Shape";
        }

        private string uintToString(uint[] num)
        {
            string str = "";
            foreach(uint i in num)
            {
                str += $"{i}{ComBridge.SeperatorKey}";
            }
            return str;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Pixels", pixels);
            info.AddValue("xOffset", xOffset);
            info.AddValue("yOffset", yOffset);
            info.AddValue("width", pixelWidth);
            info.AddValue("height", pixelHeight);
        }

        public ShapeData(SerializationInfo info, StreamingContext context)
        {
            pixels = (uint[])info.GetValue("Pixels", typeof(uint[]));
            xOffset = (int)info.GetValue("xOffset", typeof(int));
            yOffset = (int)info.GetValue("yOffset", typeof(int));
            pixelWidth = (int)info.GetValue("width", typeof(int));
            pixelHeight = (int)info.GetValue("height", typeof(int));
        }
    }
}
