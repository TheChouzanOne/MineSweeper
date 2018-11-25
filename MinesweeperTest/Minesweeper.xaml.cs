using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MinesweeperTest
{
    /// <summary>
    /// Interaction logic for Minesweeper.xaml
    /// </summary>
    public partial class Minesweeper : Window
    {

        #region "PROPIEDADES O VARIABLES GLOBALES"

        public int gridHeight { get; set; }
        public int gridWidth { get; set; }
        public int mines { get; set; }
        public int flagsLeft { get; set; }
        public int revealedCells { get; set; }
        public bool firstClick { get; set; }
		public bool gameStarted { get; set; }

        public bool lost { get; set; }

        Canvas field = new Canvas();
        Canvas information = new Canvas();
        Button btnMenu = new Button();
		BombCounter bombCounter;
		Timer timer = new Timer();
		Label lblTime = new Label();

        public List<List<Cell>> grid { get; set; }

        #endregion

        #region "CONSTRUCTOR"

        public Minesweeper(int mines, int gridWidth, int gridHeight)
        {
            grid = new List<List<Cell>>();
            lost = false;
            this.mines = mines;
            flagsLeft = mines;
            this.gridHeight = gridHeight;
            this.gridWidth = gridWidth;
            revealedCells = 0;
            firstClick = true;
			gameStarted = false;

            InitializeComponent();

            Height = 24 * (4 + gridHeight) + 31; //Estos ultimos numeros son por los bordes del window
            Width = 24 * (2 + gridWidth) + 16;
            ResizeMode = ResizeMode.NoResize;
        }
        
        #endregion

        #region "METODOS DE EVENTOS"

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            field.Height = 24 * gridHeight;
            field.Width = 24 * gridWidth;
            elCanvas.Children.Add(field);
            Canvas.SetTop(field, 3 * 24);
            Canvas.SetLeft(field, 24);

            field.MouseLeftButtonUp += Field_MouseLeftButtonUp;
            field.MouseRightButtonUp += Field_MouseRightButtonUp;
			field.MouseUp += Field_MouseUp;

            information.Height = 24 * 2;
            information.Width = field.Width;
            elCanvas.Children.Add(information);
            Canvas.SetTop(information, 24 / 2);
            Canvas.SetLeft(information, 24);

			bombCounter = new BombCounter(mines);
			information.Children.Add(bombCounter);
			Canvas.SetTop(bombCounter, 0);
			Canvas.SetLeft(bombCounter, 0);

			information.Children.Add(timer);
			Canvas.SetTop(timer, 0);
			Canvas.SetLeft(timer, information.Width - timer.Width);

			btnMenu.Height = 40;
            btnMenu.Width = 80;
            btnMenu.Content = "Menu";
            btnMenu.FontSize = 25;
            information.Children.Add(btnMenu);
            Canvas.SetTop(btnMenu, 4);
            Canvas.SetLeft(btnMenu, (information.Width / 2) - (btnMenu.Width / 2));

            btnMenu.Click += BtnMenu_Click;

            InitializeGrid(gridWidth, gridHeight);

            CreateGrid(mines, gridWidth, gridHeight);
            ShuffleGrid(); //PARA EVITAR O MINIMIZAR QUE AL FINAL QUEDEN MUCHAS CELDAS VACIAS O CELDAS CON BOMBAS JUNTITAS POR LA LOGICA DE CREATEGRID
            CountMines();
            DrawGrid(field);


			//DEBUGGING

			//timer.Start();

		}

		private void Field_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (!gameStarted)
			{
				timer.Start();
				gameStarted = true;
			}
		}

		private void BtnMenu_Click(object sender, RoutedEventArgs e)
        {
            Menu menu = new Menu();
            menu.Show();
            Close();
        }

        private void Field_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
			Cell cell = (Cell)e.Source;
            int posX = cell.posX;
            int posY = cell.posY;
            if (!cell.clicked)
            {
                if (cell.flagged)
                {
                    grid[posY][posX].Unflag();
                    flagsLeft++;
                }
                else if (flagsLeft > 0)
                {
                    grid[posY][posX].Flag();
                    flagsLeft--;
                }
            }

			bombCounter.SetCounter(flagsLeft);

            //DEBUGGING

        }

        private void Field_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Cell cell = e.Source as Cell;

            if (!cell.clicked && !cell.flagged)
            {
                RevealCells(cell.posX, cell.posY);
            }
            else
            {
                if(!cell.flagged)
                {
                    List<int> validX = new List<int>();
                    List<int> validY = new List<int>();

                    validX.Add(cell.posX);
                    validY.Add(cell.posY);

                    if (cell.posY != 0)
                    {
                        validY.Add(cell.posY - 1);
                    }

                    if (cell.posY != gridHeight - 1)
                    {
                        validY.Add(cell.posY + 1);
                    }

                    if (cell.posX != 0)
                    {
                        validX.Add(cell.posX - 1);
                    }

                    if (cell.posX != gridWidth - 1)
                    {
                        validX.Add(cell.posX + 1);
                    }

                    foreach (int posY in validY)
                    {
                        foreach (int posX in validX)
                        {
                            Cell cellToReveal = grid[posY][posX];

                            if (!(posY == cell.posY && posX == cell.posX) && !cellToReveal.flagged)
                            {
                                RevealCells(posX, posY);
                            }
                        }
                    }
                }
            }

            if (lost)
            {
                Lost();
            }

            CheckWin();
        }
        
        #endregion

        #region "METODOS DE AYUDA"

        public void DrawGrid(Canvas elCanvas)
        {
            elCanvas.Children.Clear();
            foreach (List<Cell> row in grid)
            {
                foreach(Cell cell in row)
                {
                    elCanvas.Children.Add(cell);

                    Canvas.SetLeft(cell, cell.posX * 24);
                    Canvas.SetTop(cell, cell.posY * 24);
                }
            }
        }

        public void InitializeGrid(int width, int height)
        {
            for (int x = 0; x < height; x++)
            {
                grid.Add(new List<Cell>(width));
            }
        }

        public void CreateGrid(int mines, int width, int height)
        {
            Random rng = new Random();
            int minesLeft = mines;
            int safeCellsLeft = width * height - mines;

            for (int y=0; y<height; y++)
            {
                for(int x=0; x< width; x++)
                {
                    int choice = rng.Next( width * height); //PARA QUE SE DISTRIBUYAN POR TODO EL GRID, NECESITA HABER UNA PROBABILIDAD APROPIADA A LA RAZON DE CELDAS SEGURAS Y MINAS
                    if (choice < mines - 1) //MINE
                    {
                        if (minesLeft>0)
                        {
                            grid[y].Add(new Mine());
                            minesLeft--;
                        }
                        else
                        {
                            grid[y].Add(new SafeCell());
                        }
                    }
                    else //SAFECELL
                    {
                        if (safeCellsLeft > 0)
                        {
                            grid[y].Add(new SafeCell());
                            safeCellsLeft--;
                        }
                        else
                        {
                            grid[y].Add(new Mine());
                        }
                    }

                    grid[y][x].posX = x;
                    grid[y][x].posY = y;
                }
            }


        }

        public void ShuffleGrid()
        {
            Random rng = new Random();

            for(int x=0; x< gridWidth*gridHeight; x++)
            {
                int x1 = rng.Next(gridWidth);
                int x2 = rng.Next(gridWidth);
                int y1 = rng.Next(gridHeight);
                int y2 = rng.Next(gridHeight);

                Cell cell1 = grid[y1][x1];
                Cell cell2 = grid[y2][x2];

                int posX1 = cell1.posX;
                int posY1 = cell1.posY;

                cell1.posX = cell2.posX;
                cell1.posY = cell2.posY;

                cell2.posX = posX1;
                cell2.posY = posY1;

                grid[y1][x1] = cell2;
                grid[y2][x2] = cell1;
            }
        }

        private void CountMines()
        {

            foreach (List<Cell> row in grid) //RESETEAR LAS MINAS A 0
            {
                foreach (Cell cell in row)
                {
                    if (!cell.isMine)
                    {
                        SafeCell safecell = (SafeCell)cell;
                        safecell.bombsNearby = 0;
                    }
                }
            }

            List<int> validX = new List<int>();
            List<int> validY = new List<int>();

            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    if (grid[y][x].isMine)
                    {
                        validX.Add(x);
                        validY.Add(y);

                        if (y != 0)
                        {
                            validY.Add(y - 1);
                        }

                        if (y != gridHeight - 1)
                        {
                            validY.Add(y + 1);
                        }
                        if (x != 0)
                        {
                            validX.Add(x - 1);
                        }

                        if (x != gridWidth - 1)
                        {
                            validX.Add(x + 1);
                        }

                        foreach (int posX in validX)
                        {
                            foreach (int posY in validY)
                            {
                                if (!(posY == y && posX == x))
                                {
                                    Cell cell = grid[posY][posX];
                                    if (!cell.isMine)
                                    {
                                        SafeCell safeCell = (SafeCell)cell;
                                        safeCell.bombsNearby++;
                                    }
                                }

                            }
                        }

                        validX = new List<int>();
                        validY = new List<int>();
                    }
                }
            }
        }

        public void RevealCells(int x, int y)
        {
            Cell cellTemp = grid[y][x];
            if (!cellTemp.isMine)
            {
                SafeCell cell = (SafeCell)cellTemp;

                if (!cell.clicked)
                {
                    List<int> validX = new List<int>();
                    List<int> validY = new List<int>();

                    validX.Add(x);
                    validY.Add(y);

                    if (y != 0)
                    {
                        validY.Add(y - 1);
                    }

                    if (y != gridHeight - 1)
                    {
                        validY.Add(y + 1);
                    }

                    if (x != 0)
                    {
                        validX.Add(x - 1);
                    }

                    if (x != gridWidth - 1)
                    {
                        validX.Add(x + 1);
                    }

                    cell.Reveal();

					if (!cell.flagged)
					{
						revealedCells++;
						cell.clicked = true;
					}


                    if (cell.bombsNearby == 0)
                    {
                        foreach (int posY in validY)
                        {
                            foreach (int posX in validX)
                            {
                                if (!(posY == y && posX == x))
                                {
                                    RevealCells(posX, posY);
                                }
                            }
                        }
                    }

                }

            }
            else
            {
                lost = true;
                Mine mine = (Mine)cellTemp;
                mine.clicked = true;
                mine.exploded = true;
            }
        }

        public void CheckWin()
        {
            if(revealedCells == gridHeight*gridWidth - mines && !lost)
            {
				timer.Stop();
				flagsLeft = 0;
				bombCounter.SetCounter(flagsLeft);
				foreach (List<Cell> row in grid)
                {
                    foreach(Cell cell in row)
                    {
                        if (cell.isMine)
                        {
                            cell.Flag();
                        }
                    }
                }

                field.IsEnabled = false;

                MessageBox.Show("You've won in " + timer.Time + " seconds!");
            }
        }

        public void Lost()
        {
            lost = true;
			timer.Stop();
            foreach (List<Cell> row in grid)
            {
                foreach (Cell cell in row)
                {
                    cell.LostReveal();

                    if (cell.isMine)
                    {
                        Mine mine = (Mine)cell;
                        if (mine.exploded)
                        {
                            mine.Explode();
                        }
                    }
                }
            }

            MessageBox.Show("You've lost!");

            field.IsEnabled = false;
        }
        #endregion

        private void elCanvas_KeyUp(object sender, KeyEventArgs e)
        {
            
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.X)
            {
                for (int i = 0; i < gridHeight; i++)
                {
                    for (int x = 0; x < gridWidth; x++)
                    {
                        if (grid[i][x].isMine)
                        {
                            grid[i][x].Flag();
                            grid[i][x].flagged = true;
                            
                        }
                    }
                }
            }
        }
    }
}
