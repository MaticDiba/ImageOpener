using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.IO;
using AForge.Imaging;

namespace ImageOpener
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public System.Drawing.Color Color2Search { get; private set; }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                imgPhoto.Source = new BitmapImage(new Uri(op.FileName));
            }
        }

        private void imgPhoto_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point position = e.GetPosition(imgPhoto);

            double controlWidth = Width;
            double controlHeight = Height;
            double imageWidth = imgPhoto.Width;
            double imageHeight = imgPhoto.Height;
            BitmapImage source = (BitmapImage)imgPhoto.Source;
            Bitmap bitmap = ImageProcessor.ImageProcessor.BitmapImage2Bitmap(source);
            System.Drawing.Color pixel = bitmap.GetPixel(
               (int)(position.X),
               (int)(position.Y));
            this.Color2Search = pixel;
            System.Diagnostics.Debug.WriteLine(pixel);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            ImageProcessor.ImageProcessor processor = new ImageProcessor.ImageProcessor((BitmapImage)imgPhoto.Source);
            var allBlobs = ImageProcessor.ImageProcessor.GetBlobs((BitmapImage)imgPhoto.Source);

            //var allPixles = processor.FindColor(this.Color2Search);

            FillCircles2Canvas(allBlobs);
            //FillPixles2Canvas(allPixles);
        }

        private void FillCircles2Canvas(Blob[] allBlobs)
        {

            foreach (Blob pojnt in allBlobs)
            {
                Ellipse ellipse = new Ellipse();
                ellipse.Fill = System.Windows.Media.Brushes.Blue;
                ellipse.Width = 2;
                ellipse.Height = 2;
                ellipse.StrokeThickness = 2;

                Cnv.Children.Add(ellipse);

                Canvas.SetLeft(ellipse, pojnt.CenterOfGravity.X);
                Canvas.SetTop(ellipse, pojnt.CenterOfGravity.Y);
            }
        }

        private void FillPixles2Canvas(System.Drawing.Point[] allPixles)
        {
            foreach (System.Drawing.Point pojnt in allPixles)
            {
                Ellipse ellipse = new Ellipse();
                ellipse.Fill = System.Windows.Media.Brushes.Blue;
                ellipse.Width = 2;
                ellipse.Height = 2;
                ellipse.StrokeThickness = 2;

                Cnv.Children.Add(ellipse);

                Canvas.SetLeft(ellipse, pojnt.X);
                Canvas.SetTop(ellipse, pojnt.Y);
            }
        }

        private void buttoFiltern_Click(object sender, RoutedEventArgs e)
        {
            Bitmap bitmap = ImageProcessor.ImageProcessor.FilterImage((BitmapImage)imgPhoto.Source);
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();

            imgPhoto.Source = bi;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            HoughCircle[] circles = ImageProcessor.ImageProcessor.ApplyHoughCircle((BitmapImage)imgPhoto.Source);
            foreach (HoughCircle circle in circles)
            {
                Ellipse ellipse = new Ellipse();
                ellipse.Fill = System.Windows.Media.Brushes.Blue;
                ellipse.Width = 2;
                ellipse.Height = 2;
                ellipse.StrokeThickness = 2;

                Cnv.Children.Add(ellipse);

                Canvas.SetLeft(ellipse, circle.X);
                Canvas.SetTop(ellipse, circle.Y);
            }
        }
    }
}
