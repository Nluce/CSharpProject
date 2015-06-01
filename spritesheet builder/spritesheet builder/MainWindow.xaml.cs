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

    public class SpriteRecord
    {
        // holde the sprite image
        public BitmapSource spriteImage;

        // holds the position of the sprite on the sprite sheet
        public Rect rect;

        public static int CompareByHeight(SpriteRecord x, SpriteRecord y){
            return -x.spriteImage.PixelHeight.CompareTo (y.spriteImage.PixelHeight);
        }


        internal void setPosition(int x, int y)
        {
            rect.X = x;
            rect.Y = y;
            rect.Width = spriteImage.PixelWidth;
            rect.Height = spriteImage.PixelHeight;
        }
    }

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



            List<SpriteRecord> spriteList = new List<SpriteRecord>();
            

            foreach(string file in filenames){
                // Open a Stream and decode a PNG image
                Stream imageStreamSource = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                PngBitmapDecoder decoder = new PngBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                BitmapSource bitmapSource = decoder.Frames[0];
                Console.Out.WriteLine(file);
                Console.Out.WriteLine(bitmapSource.PixelWidth);
                Console.Out.WriteLine(bitmapSource.PixelHeight);

                SpriteRecord spriteRecord = new SpriteRecord();
                spriteRecord.spriteImage = bitmapSource;
                spriteList.Add(spriteRecord);
            }

            spriteList.Sort(SpriteRecord.CompareByHeight);

            const int sheetWidth = 128;
            const int sheetHeight = 256;

            int x = 0; 
            int y = 0;
            int nextRow = y;

            foreach (SpriteRecord spriteRecord in spriteList)
            {


                spriteRecord.setPosition(x, y);

                if (x + spriteRecord.spriteImage.PixelWidth >= sheetWidth)
                {
                    x = 0;
                    y = nextRow;
                    spriteRecord.setPosition(x, y);

                }

                nextRow = Math.Max(y + spriteRecord.spriteImage.PixelHeight, nextRow);

                x += spriteRecord.spriteImage.PixelWidth;

                drawingContext.DrawImage(spriteRecord.spriteImage, spriteRecord.rect);

       


            }


            drawingContext.Close();

            RenderTargetBitmap bmp = new RenderTargetBitmap(256, 256, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);

            ImageArea.Source = bmp;
            ImageArea.Stretch = Stretch.None;

        } 
    }
}
