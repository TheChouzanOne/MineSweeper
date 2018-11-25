using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MinesweeperTest
{
    class Mine : Cell
    {

        #region URIS
        Uri explodedMineCellUri = new Uri(@"/images/ExplodedMineCell.png", UriKind.Relative);
        Uri mineUri = new Uri(@"/images/Mine.png", UriKind.Relative);
        Uri revealedMineCellUri = new Uri(@"/images/RevealedMineCell.png", UriKind.Relative);
        #endregion

        public bool exploded { get; set; }

        public Mine():base(true)
        {
            exploded = false;
        }

        public override void LeftButtonUp()
        {
            Reveal();
        }

        public override void Reveal()
        {
            if (!flagged)
            {
                image.Source = new BitmapImage(revealedMineCellUri);
            }
        }

		public override void LostReveal()
		{
			Reveal();
		}

		public void Explode()
        {
            image.Source = new BitmapImage(explodedMineCellUri);
        }
    }
}
