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
using System.Xml.Linq;


using Microsoft.Win32;

namespace spritesheet_builder
{

    public class SpriteRecord
    {
        // holde the sprite image
        public BitmapSource spriteImage;

        // holds the position of the sprite on the sprite sheet
        public Rect rect;

        public string FileName;

        public static int CompareByHeight(SpriteRecord x, SpriteRecord y){
            return -x.spriteImage.PixelHeight.CompareTo(y.spriteImage.PixelHeight);
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
        List<SpriteRecord> SpriteList = new List<SpriteRecord>();



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

            SpriteList = new List<SpriteRecord>();

            foreach(string file in filenames){
                // Open a Stream and decode a PNG image
                Stream imageStreamSource = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                PngBitmapDecoder decoder = new PngBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                BitmapSource bitmapSource = decoder.Frames[0];

                SpriteRecord spriteRecord = new SpriteRecord();
                spriteRecord.spriteImage = bitmapSource;
                spriteRecord.FileName = System.IO.Path.GetFileName(file);
                SpriteList.Add(spriteRecord);
            }

            SpriteList.Sort(SpriteRecord.CompareByHeight);

            const int sheetWidth = 256;
            const int sheetHeight = 256;

            int x = 0; 
            int y = 0;
            int nextRow = y;

            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            foreach (SpriteRecord spriteRecord in SpriteList)
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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "SpriteSheet.png";
            dlg.DefaultExt = ".png"; // Default file extension 
            dlg.Filter = "Png Images (.png)|*.png"; // Filter files by extension 
            // Show open file dialog box 
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true) {
                using (var fileStream = new FileStream(dlg.FileName, FileMode.Create))
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(ImageArea.Source as BitmapSource));
                    encoder.Save(fileStream);
                }


                SaveXML(dlg.FileName);
            }



        }

        private void SaveXML(string fileName)
        {
            var root = new XElement("SpriteSheet");

            root.Add(new XAttribute("FileName", System.IO.Path.GetFileName(fileName)));

            foreach (SpriteRecord spriteRecord in SpriteList)
            {
                var spriteElement = new XElement("Sprite", new XAttribute("FileName", spriteRecord.FileName));
                spriteElement.Add(new XAttribute("Rectangle", spriteRecord.rect));
                root.Add(spriteElement);
            }

            root.Save(fileName + ".xml");

        } 
    }
}
