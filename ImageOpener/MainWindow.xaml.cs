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
using AForge.Imaging.Filters;

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
            button2.IsEnabled = true;
            button_Copy.IsEnabled = true;
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
            /*
            Bitmap bitmap = ImageProcessor.ImageProcessor.FilterImage((BitmapImage)imgPhoto.Source);
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();
            */
            Bitmap bitmap = ImageProcessor.ImageProcessor.HSL((BitmapImage)imgPhoto.Source);
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();
            imgPhotoHugh.Source = bi;// imgPhoto.Source;
            sliderFilterLow.IsEnabled = true;
            sliderFilterHigh.IsEnabled = true;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            int radius = Convert.ToInt32(slider.Value);
            int tresh = Convert.ToInt32(sliderTresh.Value);
            HoughCircleTransformation circles = ImageProcessor.ImageProcessor.ApplyHoughCircle((BitmapImage)imgPhoto.Source, radius, tresh);
            double houghWidthFactor = imgPhoto.ActualHeight/ imgPhoto.Source.Height;
            double houghHeightFactor = imgPhoto.ActualWidth/ imgPhoto.Source.Width;
            /*
            foreach (HoughCircle circle in circles.OrderBy(circle => circle.Intensity).Take(1000))
            {
                Ellipse ellipse = new Ellipse();
                ellipse.Fill = System.Windows.Media.Brushes.Blue;
                ellipse.Width = 10;
                ellipse.Height = 10;
                ellipse.StrokeThickness = 2;
                ellipse.SetValue(Canvas.LeftProperty, (double)circle.X*houghWidthFactor);
                ellipse.SetValue(Canvas.TopProperty, (double)circle.Y*houghHeightFactor);

                CnvHugh.Children.Add(ellipse);
            }*/
            //CnvHugh.Background = new ImageBrush(imgPhoto.Source);
            MemoryStream ms = new MemoryStream();
            circles.ToBitmap().Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();
            imgPhotoHugh.Source = bi;
        }

        private void buttonTresh_Click(object sender, RoutedEventArgs e)
        {
            int tresh = Convert.ToInt32(sliderTresh.Value);
            Bitmap tresholdedPicture = ImageProcessor.ImageProcessor.ApplyFilter((BitmapImage)imgPhoto.Source, new Threshold(tresh));

            MemoryStream ms = new MemoryStream();
            tresholdedPicture.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();

            imgPhotoHugh.Source = bi;
        }

        private void sliderTresh_ValueChanged(object sender, RoutedEventArgs e)
        {
            Slider slider = sender as Slider;
            double low = sliderFilterLow.Value;
            double high = sliderFilterHigh.Value;
            if(low > high)
            {
                high = low + 1;
                sliderFilterHigh.Value = high;
            }
            Bitmap bitmap = ImageProcessor.ImageProcessor.FilterImage((BitmapImage)imgPhoto.Source, low, high);
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();

            imgPhotoHugh.Source = bi;
            sliderFilterLow.IsEnabled = true;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            imgPhoto.Source = imgPhotoHugh.Source;
        }
    }
}
