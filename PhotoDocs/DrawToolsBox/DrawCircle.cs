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
    public class DrawCircle : IDrawTools
    {
        private PhotoDocsManager photoDocsManager;
        private Point MouseDownPoint;
        private Point MouseUpPoint;
        private int Thickness;
        private uint ShapeColor;

        public DrawCircle(PhotoDocsManager photoDocsManager, int thickness = 1, uint color = (unchecked((uint)((255 << 24) + (255 << 16) + (0 << 8) + 0))))
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
            int width = Thickness;
            int height = Thickness;

            int cursorx = (int)MouseUpPoint.X + ComBridge.MouseOffsetX - width/2;
            int cursory = (int)MouseUpPoint.Y + ComBridge.MouseOffsetY - height/2;

            uint[] pixels = new uint[width * height];

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    int i = width * y + x;

                    //sqrt((x1-x2)^2+(y1-y2)^2)
                    //alpha, red, green, blue
                    //(uint)((alpha << 24) + (red << 16) + (green << 8) + blue)
                    if (Math.Sqrt((x - width / 2) * (x - width/2) + (y - height / 2) * (y - height / 2)) < Thickness/2)
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
