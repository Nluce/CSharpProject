using System;
using System.Collections.Generic;
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
using System.IO;


using Microsoft.Win32;

namespace spritesheet_builder
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = "Document"; // Default file name 
            dlg.DefaultExt = ".png"; // Default file extension 
            dlg.Filter = "Png Images (.png)|*.png"; // Filter files by extension 
            dlg.Multiselect = true;

            // Show open file dialog box 
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Open document 
                MakeSpriteSheet(dlg.FileNames);
            }


        }
        private void MakeSpriteSheet(string[] filenames)
        {

            if (false) { 
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.FileName = "SpriteSheet.png";
                dlg.DefaultExt = ".png"; // Default file extension 
                dlg.Filter = "Png Images (.png)|*.png"; // Filter files by extension 
                // Show open file dialog box 
                Nullable<bool> result = dlg.ShowDialog();
            }
            Image[] images = new Image[filenames.Length];

            DrawingVisual drawingVisual = new DrawingVisual();


            DrawingContext drawingContext = drawingVisual.RenderOpen();

            int x = 0; 
            int y = 0;
            

            foreach(string file in filenames){
                // Open a Stream and decode a PNG image
                Stream imageStreamSource = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                PngBitmapDecoder decoder = new PngBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                BitmapSource bitmapSource = decoder.Frames[0];

                Console.Out.WriteLine(file);
                Console.Out.WriteLine(bitmapSource.PixelWidth);
                Console.Out.WriteLine(bitmapSource.PixelHeight);
            

                Rect recked = new Rect(x, y, bitmapSource.PixelWidth, bitmapSource.PixelHeight);

                drawingContext.DrawImage(bitmapSource, recked);

                x += bitmapSource.PixelWidth;


            }

            drawingContext.Close();

            RenderTargetBitmap bmp = new RenderTargetBitmap(256, 256, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);

            ImageArea.Source = bmp;
            ImageArea.Stretch = Stretch.None;

        } 
    }
}
