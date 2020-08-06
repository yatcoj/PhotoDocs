using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using PhotoDocs.DrawToolsBox;
using SharedData;

namespace PhotoDocs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PhotoDocsManager pdm;

        private uint[] imagePixels;
        private uint[] colorPickerPixels;
        private WriteableBitmap image;

        public MainWindow()
        { 
            InitializeComponent();
            this.pdm = new PhotoDocsManager(this);
            imagePixels = new uint[(int)drawArea.Width * (int)drawArea.Height];
            image = new WriteableBitmap((int)drawArea.Width, (int)drawArea.Height, 100, 100, PixelFormats.Bgra32, null);
            this.pdm.FirstLoad();
            UsernameTextBox.Text = ConfigurationManager.AppSettings["Username"];
            SetColorPickerInitColors();
        }

        public void Update()
        {
            Dispatcher.Invoke(() =>
            {
                pdm.Update();
            });
        }

        public void UpdateCanvas(ShapeData shape)
        {
            if (shape != null)
            {
                int xOffset = shape.xOffset;
                int yOffset = shape.yOffset;
                int pixelWidth = shape.pixelWidth;
                uint[] pixels = shape.pixels;

                int width = (int)drawArea.Width;
                int height = (int)drawArea.Height;

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
                            if (i < imagePixels.Length && ip < pixels.Length)
                            {
                                if (pixels[ip] != ComBridge.DontDrawKey)
                                {
                                    imagePixels[i] = pixels[ip];
                                }
                            }
                        }
                    }
                }

                image.WritePixels(new Int32Rect(0, 0, width, height), imagePixels, width * 4, 0);

                ImageBrush brush = new ImageBrush();
                brush.ImageSource = image;
                drawArea.Background = brush;
            }
        }

        public uint[] GetPixels()
        {
            return imagePixels;
        }

        public int GetPictureWidth()
        {
            return (int)drawArea.Width;
        }

        public int GetPictureHeight()
        {
            return (int)drawArea.Height;
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            pdm.ConnectClient();
        }

        private void CanvasClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                pdm.CanvasClick(e.GetPosition(drawArea));
            }
        }

        private void CanvasUnClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState != MouseButtonState.Pressed)
            {
                pdm.CanvasUnClick(e.GetPosition(drawArea));
            }
        }

        private void CanvasDrag(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                pdm.CanvasDrag(sender, e, this);
            }
        }

        private void ColorPickerCanvasClick(object sender, MouseButtonEventArgs e)
        {
            if (colorPickerPixels != null && colorPickerPixels.Length > 0)
            {
                int width = (int)colorSquare.Width;
                int height = (int)colorSquare.Height;

                int cursorx = (int)e.GetPosition(colorSquare).X;
                int cursory = (int)e.GetPosition(colorSquare).Y;

                uint[] pixels = new uint[0];

                uint clickedColor = colorPickerPixels[width * cursory + cursorx];
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

                this.SetColor(alpha2, red2, green2, blue2);
            }
        }

        private void SetColorPickerInitColors()
        {
            int width = (int)colorSquare.Width;
            int height = (int)colorSquare.Height;
            WriteableBitmap imageC = new WriteableBitmap(width, height, 100, 100, PixelFormats.Bgra32, null);
            ImageBrush brush = new ImageBrush();

            uint[] pixels = new uint[width * height];

            int a = 255;
            int r = 255;
            int g = 0;
            int b = 0;
            for (int x = 0; x < width; ++x)
            {
                if(x < width/6)
                {
                    double d = (1.0 * x) / (width / 6);
                    g = (int)(255 * (d));
                }
                else if(x < 2 * (width / 6))
                {
                    double d = (1.0 * (x - (2* width / 6)) / (width / 6));
                    if ((1.0 - d) > 1)
                    {
                        d +=1;
                    }
                    r = (int)(255 * (1.0-d));
                    if (r > 255)
                    {
                        r = 255;
                    }
                }
                else if (x < 3*(width / 6))
                {
                    double d = (1.0*x-width/6*3 ) / ((width / 6));
                    b = (int)(255 * (d));
                }
                else if (x < 4 * (width / 6))
                {
                    double d = (1.0 * (x - (4 * width / 6)) / (width / 6));
                    if ((1.0 - d) > 1)
                    {
                        d += 1;
                    }
                    g = (int)(255 * (1.0 - d));
                    if (g > 255)
                    {
                        g = 255;
                    }
                }
                else if (x < 5 * (width / 6))
                {
                    double d = (1.0 * x - width / 6 * 5) / ((width / 6));
                    r = (int)(255 * (d));
                    if (r > 255)
                    {
                        r = 255;
                    }
                }
                else if (x < 6 * (width / 6))
                {
                    double d = (1.0 * (x - (6 * width / 6)) / (width / 6));
                    if ((1.0 - d) > 1)
                    {
                        d += 1;
                    }
                    b = (int)(255 * (1.0 - d));
                    if (b > 255)
                    {
                        b = 255;
                    }
                }

                for (int y = 0; y < height; ++y)
                {
                    int i = width * y + x;
                    //alpha, red, green, blue
                    pixels[i] = (uint)((a << 24) + (r << 16) + (g << 8) + b);
                }
            }
            imageC.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 4, 0);
            brush.ImageSource = imageC;
            colorSquare.Background = brush;
            colorPickerPixels = pixels;
        }

        private void Open_Pic_Clicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            
            int width = (int)drawArea.Width;
            int height = (int)drawArea.Height;
            pdm.OpenPic(openFileDialog.FileName, width, height);
        }

        private void Save_Pic_Clicked(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sFileDialog = new SaveFileDialog();
            sFileDialog.ShowDialog();
            pdm.SavePic(image, sFileDialog.FileName);
        }

        private void Set_ToolSquare(object sender, RoutedEventArgs e)
        {
            pdm.SetTool(new DrawSquare(pdm, (int)ThicknessSlider.Value, GetCurrentColor()));
        }

        private void Set_ToolCircle(object sender, RoutedEventArgs e)
        {
            pdm.SetTool(new DrawCircle(pdm, (int)ThicknessSlider.Value, GetCurrentColor()));
        }

        private void Set_ToolSpray(object sender, RoutedEventArgs e)
        {
            pdm.SetTool(new SprayTool(pdm, (int)ThicknessSlider.Value, GetCurrentColor()));
        }

        private void ThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (pdm != null)
            {
                pdm.SetThickness(ThicknessSlider.Value);
                ThicknessNumber.Content = "" + (int)ThicknessSlider.Value;
            }
        }

        private void Set_ToolBrush(object sender, RoutedEventArgs e)
        {
            pdm.SetTool(new BrushTool(pdm, (int)ThicknessSlider.Value, GetCurrentColor()));
        }

        private void Set_ToolPaintBucket(object sender, RoutedEventArgs e)
        {
            pdm.SetTool(new PaintBucket(pdm, (int)ThicknessSlider.Value, GetCurrentColor()));
        }

        private void JoinLobby_Clicked(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string str = $"{ComBridge.JoinDocKeyword}{ComBridge.SeperatorKey}" +
                $"{username}{ComBridge.SeperatorKey}" +
                $"{LobbyTextBox.Text}{ComBridge.SeperatorKey}";

            pdm.ConnectClient();
            pdm.SendMsg(str);
        }

        private void HostLobby_Clicked(object sender, RoutedEventArgs e)
        {
            int width = (int)drawArea.Width;
            int height = (int)drawArea.Height;

            string username = UsernameTextBox.Text;
            string str = $"{ComBridge.HostDocKeyword}{ComBridge.SeperatorKey}" +
                $"{username}{ComBridge.SeperatorKey}" +
                $"{width}{ComBridge.SeperatorKey}" +
                $"{height}{ComBridge.SeperatorKey}";

            pdm.ConnectClient();
            pdm.SendMsg(str);
        }

        private bool SetColor()
        {
            try
            { 
                pdm.SetColor(GetCurrentColor());
                UpdateColorImage(GetCurrentColor());
            }
            catch(Exception e)
            {
                return false;
            }

            return true;
        }

        private void UpdateColorImage(uint col)
        {
            int width = (int)SelectedColorImage.Width;
            int height = (int)SelectedColorImage.Height;
            WriteableBitmap imageC = new WriteableBitmap(width, height, 100, 100, PixelFormats.Bgra32, null);
            ImageBrush brush = new ImageBrush();

            uint[] pixels = new uint[width * height];

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    int i = width * y + x;

                    //alpha, red, green, blue
                    pixels[i] = col;
                }
            }
            imageC.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 4, 0);
            brush.ImageSource = imageC;
            SelectedColorImage.Background = brush;
        }

        private uint GetCurrentColor()
        {
            //alpha, red, green, blue
            int r = int.Parse(rgb_red.Text);
            int b = int.Parse(rgb_blue.Text);
            int g = int.Parse(rgb_green.Text);
            int a = int.Parse(rgb_alpha.Text);
            uint col = (uint)((a << 24) + (r << 16) + (g << 8) + b);
            return col;
        }

        public void SetColor(int a, int r, int g, int b)
        {
            rgb_red.Text = r + "";
            rgb_blue.Text = b + "";
            rgb_green.Text = g + "";
            rgb_alpha.Text = a+"";
            UpdateColorImage(GetCurrentColor());
        }

        private void TestingThreadSafeDrawing(object sender, RoutedEventArgs e)
        {
            System.Threading.Thread incomingMsgManager = new System.Threading.Thread(() => FireCurrentToolThread());
            incomingMsgManager.Start();
        }

        private void FireCurrentToolThread()
        {
            /*while (true)
            {
                pdm.FireCurrentTool(100, 300);
            }*/
        }

        private void Rgb_red_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetColor();
        }

        private void Rgb_green_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetColor();
        }

        private void Rgb_blue_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetColor();
        }

        private void Rgb_alpha_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetColor();
        }

        private void rgbScroll(TextBox tb, MouseWheelEventArgs e)
        {
            try
            {
                int r = int.Parse(tb.Text);
                if (e.Delta > 0 && r >= 0 && r < 255)
                {
                    tb.Text = "" + (r + 1);
                }
                else if (e.Delta < 0 && r > 0 && r <= 255)
                {
                    tb.Text = "" + (r - 1);
                }
            }
            catch (Exception exc)
            {
                exc.ToString();
            }
        }

        private void Rgb_red_TextChanged_Scroll(object sender, MouseWheelEventArgs e)
        {
            rgbScroll(rgb_red, e);
        }

        private void Rgb_green_TextChanged_Scroll(object sender, MouseWheelEventArgs e)
        {
            rgbScroll(rgb_green, e);
        }

        private void Rgb_blue_TextChanged_Scroll(object sender, MouseWheelEventArgs e)
        {
            rgbScroll(rgb_blue, e);
        }

        private void Rgb_alpha_TextChanged_Scroll(object sender, MouseWheelEventArgs e)
        {
            rgbScroll(rgb_alpha, e);
        }

        private void Set_ToolColorPicker(object sender, RoutedEventArgs e)
        {
            pdm.SetTool(new ColorPickerTool(pdm, (int)ThicknessSlider.Value, GetCurrentColor()));
        }

        private void Set_ToolEraser(object sender, RoutedEventArgs e)
        {
            pdm.SetTool(new EraserTool(pdm, (int)ThicknessSlider.Value, GetCurrentColor()));
        }

        private void ChangeServerIp_Clicked(object sender, RoutedEventArgs e)
        {
            pdm.ChangeIp(ServerIP.Text);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
            pdm.ExitApplication();
        }
    }
}
