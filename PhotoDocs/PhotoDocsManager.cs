using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PhotoDocs.DrawToolsBox;
using SharedData;
using System.IO;

namespace PhotoDocs
{
    public class PhotoDocsManager
    {
        Tcp tcpClient = null;
        MainWindow mw;

        IDrawTools currentTool;

        private Queue<ShapeData> shapes = new Queue<ShapeData>();
        private Queue<string> recevedShapes = new Queue<string>();
        private string massMessage = "";

        public PhotoDocsManager(MainWindow mw)
        {
            this.mw = mw;
            //currentTool = new DrawLine(this, 20);
            currentTool = new BrushTool(this, 20);
        }

        public void Update()
        {
            while(recevedShapes.Count > 0)
            {
                string shapeString = recevedShapes.Dequeue();
                shapes.Enqueue(new ShapeData(shapeString));
            }

            while(shapes.Count > 0)
            {
                mw.UpdateCanvas(shapes.Dequeue());
            }
        }

        public uint[] GetPixels()
        {
            return mw.GetPixels();
        }

        public int GetPictureWidth()
        {
            return mw.GetPictureWidth();
        }

        public int GetPictureHeight()
        {
            return mw.GetPictureHeight();
        }

        public void ConnectClient()
        {
            tcpClient = new Tcp("127.0.0.1", 7117, this);
            tcpClient.TcpClient();
        }

        public void ChangeIp(string ip)
        {
            tcpClient.UpdateIp(ip);
        }

        public void RecevedMsg(ShapeData shape)
        {
            //There might be a problem with a new person joining as the server is sending out an update to all clients. But it should not show up unless the timeing is really bad
            shapes.Enqueue(shape);
            mw.Update();
        }

        public void DrawThis(ShapeData shape)
        {
            shapes.Enqueue(shape);
            mw.Update();
        }

        public void SendObject(ShapeData str)
        {
            if (tcpClient != null)
            {
                tcpClient.SendObject(str);
            }
        }

        public void SendMsg(string str)
        {
            tcpClient.SendMsg(str);
        }

        public void CanvasDrag(object sender, MouseEventArgs e, MainWindow mainW)
        {
            currentTool.OnDrag(e.GetPosition(mainW.drawArea));
        }

        public void CanvasClick(Point point)
        {
            currentTool.OnMouseDown(point);
        }

        public void CanvasUnClick(Point point)
        {
            currentTool.OnMouseUp(point);
        }

        public void Broadcast(ShapeData msg)
        {
            SendObject(msg);
        }

        public void SetTool(IDrawTools tool)
        {
            currentTool = tool;
        }

        public void SetThickness(double value)
        {
            currentTool.SetThickness((int)value);
        }

        public void SetColor(uint col)
        {
            currentTool.SetColor(col);
        }

        public void SelectColor(int a, int r, int g, int b)
        {
            mw.SetColor(a, r, g, b);
        }

        internal void OpenPic(string fileName, int width, int height)
        {
            //This really should be its own tool, just like a drawCircle or BrushTool
            BitmapImage writeableBitmap = new BitmapImage(new Uri(fileName, UriKind.Relative));
            WriteableBitmap bitmap = new WriteableBitmap(writeableBitmap);
            uint[] p1 = new uint[bitmap.PixelWidth * bitmap.PixelHeight];
            bitmap.CopyPixels(p1, 4 * bitmap.PixelWidth, 0);

            //WriteableBitmap bitmap = new WriteableBitmap(width, height, 100, 100, PixelFormats.Bgra32, null);
            uint[] pixels = new uint[width * height];

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    int i = width * y + x;
                    int x1 = (int)(x * bitmap.Width / width);
                    int y1 = (int)(y * bitmap.Height / height);
                    int p1i = bitmap.PixelWidth * y1 + x1;

                    //alpha, red, green, blue
                    pixels[i] = p1[p1i];
                }
            }

            bitmap = new WriteableBitmap(width, height, 100, 100, PixelFormats.Bgra32, null);
            // apply pixels to bitmap
            bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 4, 0);
            ShapeData sd = new ShapeData(pixels, 0, 0, width, height);
            this.DrawThis(sd);
            this.Broadcast(sd);
        }

        internal void SavePic(WriteableBitmap bm, string filePath)
        {
            //This really should be its own tool, just like a drawCircle or BrushTool
            if (filePath != string.Empty)
            {
                using (FileStream stream5 = new FileStream(filePath, FileMode.Create))
                {
                    PngBitmapEncoder encoder5 = new PngBitmapEncoder();
                    encoder5.Frames.Add(BitmapFrame.Create(bm));
                    encoder5.Save(stream5);
                }
            }
        }

        public void FireCurrentTool(int x, int y)
        {
            int i = 1;
            int a = 0;
            while (a<1000)
            {
                a++;
                Point p = new Point(x, y);
                if (x > 500)
                {
                    i = -1;
                }
                else if (x < 100)
                {
                    i = 1;
                }
                x += i;
                currentTool.OnMouseUp(p);
            }
        }

        public void FirstLoad()
        {
            uint color = (unchecked((uint)((255 << 24) + (255 << 16) + (255 << 8) + 255)));
            PaintBucket pb = new PaintBucket(this, 1, color);
            pb.ChangeAll();
        }

        public void ExitApplication()
        {
            tcpClient.Close();
        }
    }
}
