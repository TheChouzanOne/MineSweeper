using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MinesweeperTest
{
    class SafeCell : Cell
    {

        #region URIS
        Uri emptyCellUri = new Uri(@"/images/EmptyCell.png", UriKind.Relative);
        Uri flaggedWrongCellUri = new Uri(@"/images/FlaggedWrongCell.png", UriKind.Relative);
        Dictionary<int, Uri> numberUris = new Dictionary<int, Uri>()
        {
            {1, new Uri(@"/images/1.png", UriKind.Relative) },
            {2, new Uri(@"/images/2.png", UriKind.Relative) },
            {3, new Uri(@"/images/3.png", UriKind.Relative) },
            {4, new Uri(@"/images/4.png", UriKind.Relative) },
            {5, new Uri(@"/images/5.png", UriKind.Relative) },
            {6, new Uri(@"/images/6.png", UriKind.Relative) },
            {7, new Uri(@"/images/7.png", UriKind.Relative) },
            {8, new Uri(@"/images/8.png", UriKind.Relative) },
        };
        #endregion
        
        public int bombsNearby { get; set; }

        public SafeCell() : base(false)
        {
            bombsNearby = 0;
        }

        public override void LeftButtonUp()
        {
            if (bombsNearby == 0)
            {
                image.Source = new BitmapImage(emptyCellUri);
            }
            else
            {
                image.Source = new BitmapImage(numberUris[bombsNearby]);
            }
        }

		public override void LostReveal()
		{
			if (flagged)
			{
				image.Source = new BitmapImage(flaggedWrongCellUri);
			}
			else
			{
				if (bombsNearby == 0)
				{
					image.Source = new BitmapImage(emptyCellUri);
				}
				else
				{
					image.Source = new BitmapImage(numberUris[bombsNearby]);
				}
			}
		}

		public override void Reveal()
        {
            if (!flagged){
                if (bombsNearby == 0)
                {
                    image.Source = new BitmapImage(emptyCellUri);
                }
                else
                {
                    image.Source = new BitmapImage(numberUris[bombsNearby]);
                }
            } 
        }
    }
}
