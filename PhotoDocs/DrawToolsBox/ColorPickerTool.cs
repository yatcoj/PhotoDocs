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
    public class ColorPickerTool : IDrawTools
    {
        private PhotoDocsManager photoDocsManager;
        private Point MouseDownPoint;
        private Point MouseUpPoint;
        private int Thickness;
        private uint ShapeColor;

        private ShapeData sh;

        public ColorPickerTool(PhotoDocsManager photoDocsManager, int thickness = 1, uint color = (unchecked((uint)((255 << 24) + (255 << 16) + (0 << 8) + 0))))
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
        }

        private ShapeData CreateShape(uint[] masterImagePixels)
        {
            int width = photoDocsManager.GetPictureWidth();
            int height = photoDocsManager.GetPictureHeight();

            int cursorx = (int)MouseDownPoint.X + ComBridge.MouseOffsetX;
            int cursory = (int)MouseDownPoint.Y + ComBridge.MouseOffsetY;

            uint[] pixels = new uint[0];

            uint clickedColor = masterImagePixels[width * cursory + cursorx];
            //uint col = (uint)((a << 24) + (r << 16) + (g << 8) + b);
            uint alpha = (clickedColor);
            uint alpha1 = (((16777215 + 1) << 8) + alpha >> 24);
            int alpha2 = (int)alpha1;

            uint red = (clickedColor << 8);
            uint red1 = (((16777215 + 1) << 8) + red >> 24);
            int red2 = (int)red1;

            uint green = (clickedColor << 16);
            uint green1 = (((16777215 + 1) << 8) + green >> 24);
            int green2 = (int)green1;

            uint blue = (clickedColor << 24);
            uint blue1 = (((16777215 + 1) << 8) + blue >> 24);
            int blue2 = (int)blue1;


            photoDocsManager.SelectColor(alpha2, red2, green2, blue2);

            return new ShapeData(pixels, 0, 0, width, height);
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
