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
    public class PaintBucket : IDrawTools
    {
        private PhotoDocsManager photoDocsManager;
        private Point MouseDownPoint;
        private Point MouseUpPoint;
        private int Thickness;
        private uint ShapeColor;

        private ShapeData sh;

        public PaintBucket(PhotoDocsManager photoDocsManager, int thickness = 1, uint color = (unchecked((uint)((255 << 24) + (255 << 16) + (0 << 8) + 0))))
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
            sh = CreateShape(photoDocsManager.GetPixels());
            photoDocsManager.Broadcast(sh);
            photoDocsManager.DrawThis(sh);
        }

        private ShapeData CreateShape(uint[] masterImagePixels)
        {
            int width = photoDocsManager.GetPictureWidth();
            int height = photoDocsManager.GetPictureHeight();

            int cursorx = (int)MouseDownPoint.X + ComBridge.MouseOffsetX;
            int cursory = (int)MouseDownPoint.Y + ComBridge.MouseOffsetY;

            uint[] pixels = new uint[width * height];

            uint clickedColor = masterImagePixels[width*cursory+cursorx];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int i = width * y + x;
                    if (masterImagePixels[i] == clickedColor)
                    {
                        pixels[i] = ShapeColor;
                    }
                    else
                    {
                        pixels[i] = ComBridge.DontDrawKey;
                    }
                }
            }
            return new ShapeData(pixels, 0, 0, width, height);
        }

        public void ChangeAll()
        {
            int width = photoDocsManager.GetPictureWidth();
            int height = photoDocsManager.GetPictureHeight();

            uint[] pixels = new uint[width * height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int i = width * y + x;
                    pixels[i] = ShapeColor;
                }
            }
            photoDocsManager.DrawThis(new ShapeData(pixels, 0, 0, width, height));
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
