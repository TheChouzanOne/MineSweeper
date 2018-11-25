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

namespace MinesweeperTest
{
    
    public abstract partial class Cell : UserControl
    {
        #region URIS
        Uri cellUri = new Uri(@"/images/Cell.png", UriKind.Relative);
        Uri cellDownUri = new Uri(@"/images/CellDown.png", UriKind.Relative);
        Uri cellOverUri = new Uri(@"/images/CellOver.png", UriKind.Relative);
        Uri flaggedCellUri = new Uri(@"/images/FlaggedCell.png", UriKind.Relative);
        #endregion

        public bool isMine { get; set; }
        public bool flagged { get; set; }
        public bool clicked { get; set; }

        public int posX { get; set; }
        public int posY { get; set; }

        public Image image { get; set; }

        public Cell(bool isMine)
        {
            InitializeComponent();
            Height = 24;
            Width = 24;
            flagged = false;

            this.isMine = isMine;
            flagged = false;
            clicked = false;
        }

        private void elCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            image = new Image();
            image.Source = new BitmapImage(cellUri);
            elCanvas.Children.Add(image);
            image.Height = elCanvas.Height;
            image.Width = elCanvas.Width;

            Canvas.SetTop(image, 0);
            Canvas.SetLeft(image, 0);
        }

        private void elCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!clicked && !flagged)
            {
                image.Source = new BitmapImage(cellUri);
            }
        }

        private void elCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
            if ( !clicked && Mouse.LeftButton == MouseButtonState.Pressed && !flagged)
            {
                image.Source = new BitmapImage(cellDownUri);
            }
            else if (!clicked && !flagged)
            {
                image.Source = new BitmapImage(cellOverUri);
            }
        }

        public virtual void elCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!clicked && !flagged)
            {
                image.Source = new BitmapImage(cellDownUri);
            }
        }

        public void elCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(!clicked && !flagged)
            {
                LeftButtonUp();
            }
        }
        public abstract void LeftButtonUp();
		public abstract void LostReveal();
        public abstract void Reveal();

        public void Flag()
        {
            image.Source = new BitmapImage(flaggedCellUri);
            flagged = true;
        }

        public void Unflag()
        {
            image.Source = new BitmapImage(cellOverUri);
            flagged = false;
        }
    }
}
