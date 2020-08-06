using SharedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PhotoDocs.DrawToolsBox
{
    public class DrawLine : IDrawTools
    {
        private PhotoDocsManager photoDocsManager;
        private Point MouseDownPoint;
        private Point MouseUpPoint;
        private int Thickness = 10;
        private uint ShapeColor;

        public DrawLine(PhotoDocsManager photoDocsManager, int thickness = 1, uint color = (unchecked((uint)((255 << 24) + (255 << 16) + (0 << 8) + 0))))
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
        }

        public void OnMouseUp(Point p)
        {
            MouseUpPoint = p;
            ShapeData sh = CreateShape();
            photoDocsManager.Broadcast(sh);
            photoDocsManager.DrawThis(sh);
        }

        private ShapeData CreateShape()
        {
            int cursorx = (int)MouseUpPoint.X + ComBridge.MouseOffsetX;
            int cursory = (int)MouseUpPoint.Y + ComBridge.MouseOffsetY;

            int cursorx1 = (int)MouseDownPoint.X + ComBridge.MouseOffsetX;
            int cursory1 = (int)MouseDownPoint.Y + ComBridge.MouseOffsetY;

            int width = cursorx1;
            int height = cursory1;

            uint[] pixels = new uint[width * height];

            double slope;
            double c;
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    if ((y - width) != 0)
                    {
                        slope = (x - height) / (y - width);
                    }
                    else
                    {
                        slope = (x - height) / (y - width+1);
                    }
                    c = (cursory) - (cursorx) * slope;

                    int i = width * y + x;

                    //alpha, red, green, blue
                    if ((cursory+y) == slope*(cursorx+x)+c)
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
