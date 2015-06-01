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
        //holds filename for sprite
        public string FileName;
        //comparison routine to sort sprites from TALLEST to SHORTEST
        public static int CompareByHeight(SpriteRecord x, SpriteRecord y){
            return -x.spriteImage.PixelHeight.CompareTo(y.spriteImage.PixelHeight);
        }

        //sets rectangle for sprite
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
        //holds a list of all the sprites
        List<SpriteRecord> SpriteList = new List<SpriteRecord>();



        public MainWindow()
        {
            InitializeComponent();
        }
        //this gets called when user pressers the "add sprites..." button
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //put up a open file dialouge box
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.FileName = "Document"; // Default file name 
            dlg.DefaultExt = ".png"; // Default file extension 
            dlg.Filter = "Png Images (.png)|*.png"; // Filter files by extension 
            dlg.Multiselect = true; // Let the  user select mutiple sprites
            
            // Show open file dialog box 
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Open document 
                MakeSpriteSheet(dlg.FileNames);
            }


        }
        //this makes the sprite sheet image from the list of sprite names
        private void MakeSpriteSheet(string[] filenames)
        {
            //clear the list of sprite names
            SpriteList = new List<SpriteRecord>();
            //loop through each file name
            foreach(string file in filenames){
                // Open a Stream and decode a PNG image
                Stream imageStreamSource = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                PngBitmapDecoder decoder = new PngBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                BitmapSource bitmapSource = decoder.Frames[0];

                //create a sprite record from the bitmap source
                SpriteRecord spriteRecord = new SpriteRecord();
                spriteRecord.spriteImage = bitmapSource;
                //save the filename of the sprite
                spriteRecord.FileName = System.IO.Path.GetFileName(file);
                //add the sprite record to the sprite list
                SpriteList.Add(spriteRecord);
            }
            //this sorts the sprites from TALLEST to SHORTEST
            SpriteList.Sort(SpriteRecord.CompareByHeight);
            //height and width of the sprite sheet
            const int sheetWidth = 256;
            const int sheetHeight = 256;
            //x and y is where the next sprite will be placed
            int x = 0; 
            int y = 0;
            //next row is where the next row of sprites will be on the y axis
            int nextRow = y;
            //used to create the sprite sheet image
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            //loop through each sprite in the sprite list
            foreach (SpriteRecord spriteRecord in SpriteList)
            {

                //set position of the sprite
                spriteRecord.setPosition(x, y);
                //check to see if the sprite goes off the end of the row
                if (x + spriteRecord.spriteImage.PixelWidth >= sheetWidth)
                {
                    //move the sprite to the next row
                    x = 0;
                    y = nextRow;
                    spriteRecord.setPosition(x, y);

                }
                //keep track of where the next row should be
                nextRow = Math.Max(y + spriteRecord.spriteImage.PixelHeight, nextRow);
                //move over the width of the sprite for the next sprite
                x += spriteRecord.spriteImage.PixelWidth;
                //draw the sprite into the drawing context
                drawingContext.DrawImage(spriteRecord.spriteImage, spriteRecord.rect);
            }
            //close the drawing context
            drawingContext.Close();
            //create a bitmap image
            RenderTargetBitmap bmp = new RenderTargetBitmap(256, 256, 96, 96, PixelFormats.Pbgra32);
            //render the sprites in the sprite sheet image
            bmp.Render(drawingVisual);
            //set image area source to the bitmap so it will show up in the window
            ImageArea.Source = bmp;
            ImageArea.Stretch = Stretch.None;

        }
        //this function clicks the save button
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //put up a save dialouge box
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "SpriteSheet.png";
            dlg.DefaultExt = ".png"; // Default file extension 
            dlg.Filter = "Png Images (.png)|*.png"; // Filter files by extension 
            // Show open file dialog box 
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true) 
            {
                //this saves out the png file for the sprite sheet
                using (var fileStream = new FileStream(dlg.FileName, FileMode.Create))
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(ImageArea.Source as BitmapSource));
                    encoder.Save(fileStream);
                }

                //save the xml file
                SaveXML(dlg.FileName);
            }



        }
        //this function saves the xml file
        private void SaveXML(string fileName)
        {
            //create a root element for the sprite sheet
            var root = new XElement("SpriteSheet");
            //save the sprite sheet file name
            root.Add(new XAttribute("FileName", System.IO.Path.GetFileName(fileName)));
            //loop through each sprite in the sprite list
            foreach (SpriteRecord spriteRecord in SpriteList)
            {
                //create an element for the sprite and add a file name attribute
                var spriteElement = new XElement("Sprite", new XAttribute("FileName", spriteRecord.FileName));
                //add a rectangle attribute
                spriteElement.Add(new XAttribute("Rectangle", spriteRecord.rect));
                //add the sprite element to the root element
                root.Add(spriteElement);
            }
            //save xml file
            root.Save(fileName + ".xml");

        } 
    }
}
