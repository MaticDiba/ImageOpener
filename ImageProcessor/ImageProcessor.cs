﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge;

namespace ImageProcessor
{
    public class ImageProcessor
    {

        public ImageProcessor(BitmapImage _image)
        {
            this.Image = _image;
        }

        public BitmapImage Image { get; private set; }

        public System.Drawing.Point[] FindColor(Color color)
        {
            int searchValue = color.ToArgb();
            List<System.Drawing.Point> result = new List<System.Drawing.Point>();
            Bitmap bmp = BitmapImage2Bitmap(this.Image);
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        if (searchValue.Equals(bmp.GetPixel(x, y).ToArgb()))
                            result.Add(new System.Drawing.Point(x, y));
                    }
                }
            }
            return result.ToArray();
        }
        public static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        public static Blob[] GetBlobs(BitmapImage bitmapImage)
        {
            Bitmap bitmap = BitmapImage2Bitmap(bitmapImage);
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.ProcessImage(bitmap);
            Blob[] blobs = blobCounter.GetObjectsInformation();

            return blobs;
        }

        public static Bitmap FilterImage(BitmapImage bitmapImage)
        {
            /*Bitmap image = BitmapImage2Bitmap(bitmapImage);
            var filter = new FiltersSequence(new IFilter[]
           {
                Grayscale.CommonAlgorithms.BT709,
                new Threshold(0x20)
           });
            var binaryImage = filter.Apply(image);*/
            ColorFiltering filter = new ColorFiltering();
            // set color ranges to keep
            filter.Red = new IntRange(125, 255);
            filter.Green = new IntRange(0, 120);
            filter.Blue = new IntRange(0, 120);
            
            return filter.Apply(image);
        }

        public static HoughCircle[] ApplyHoughCircle(BitmapImage source)
        {
            Bitmap image = BitmapImage2Bitmap(source);
            var filter = new FiltersSequence(new IFilter[]
            {
                Grayscale.CommonAlgorithms.BT709,
                new Threshold(0x40)
            });
            var binaryImage = filter.Apply(image);
            HoughCircleTransformation circleTransform = new HoughCircleTransformation(35);
            // apply Hough circle transform
            circleTransform.ProcessImage(binaryImage);
            Bitmap houghCirlceImage = circleTransform.ToBitmap();
            // get circles using relative intensity
            HoughCircle[] circles = circleTransform.GetCirclesByRelativeIntensity(0.5);

            return circles;
        }
    }
}
