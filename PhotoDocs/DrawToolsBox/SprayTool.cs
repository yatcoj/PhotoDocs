using SharedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PhotoDocs.DrawToolsBox
{
    public class SprayTool : IDrawTools
    {
        private PhotoDocsManager photoDocsManager;
        private Point MouseDownPoint;
        private Point MouseUpPoint;
        private int Thickness;
        private uint ShapeColor;
        private int counter = 0;
        private Random r = new Random();

        public SprayTool(PhotoDocsManager photoDocsManager, int thickness = 1, uint color = (unchecked((uint)((255 << 24) + (255 << 16) + (0 << 8) + 0))))
        {
            this.photoDocsManager = photoDocsManager;
            Thickness = thickness;
            ShapeColor = color;
        }

        public void OnMouseDown(Point p)
        {
            MouseDownPoint = p;
        }

        public void OnDrag(Point p)
        {
            //Hopefully this will be a temporary fix to the problem where the brush tool sends way too many messages to the server
            if (counter == 0)
            {
                MouseUpPoint = p;
                ShapeData sh = CreateShape();
                photoDocsManager.Broadcast(sh);
                photoDocsManager.DrawThis(sh);
                counter = 1;
            }
            else if (counter < 3)
            {
                counter++;
            }
            else
            {
                counter = 0;
            }
        }

        public void OnMouseUp(Point p)
        {
        }

        private ShapeData CreateShape()
        {
            int width = Thickness;
            int height = Thickness;

            int cursorx = (int)MouseUpPoint.X + ComBridge.MouseOffsetX - width / 2;
            int cursory = (int)MouseUpPoint.Y + ComBridge.MouseOffsetY - height / 2;

            uint[] pixels = new uint[width * height];
            int rand = r.Next(0, 10);

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    int i = width * y + x;

                    //sqrt((x1-x2)^2+(y1-y2)^2)
                    //alpha, red, green, blue
                    //(uint)((alpha << 24) + (red << 16) + (green << 8) + blue)
                    rand = r.Next(0, 35);

                    if (rand < 5 && Math.Sqrt((x - width / 2) * (x - width / 2) + (y - height / 2) * (y - height / 2)) < Thickness / 2)
                    {
                        pixels[i] = ShapeColor;
                    }
                    else
                    {
                        pixels[i] = ComBridge.DontDrawKey;
                    }
                }
            }

            return new ShapeData(pixels, cursorx, cursory, width, height);
        }

        public void SetThickness(int thickness)
        {
            this.Thickness = thickness;
        }

        public void SetColor(uint col)
        {
            this.ShapeColor = col;
        }
    }
}
