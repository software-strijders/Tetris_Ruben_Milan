using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TetrisClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Representation _representation;
        private Tetromino _tetromino;
        private Tetromino _nextTetromino;
        private Score _score;
        private DispatcherTimer _dpt;
        private bool _paused; // Default value is false

        /// <summary>
        /// Initializes the component and the timer, then creates the first next Tetromino
        /// so it can be used in the NewTetromino method.
        /// After that it renders the new next tetromino and renders the board with the first tetromino.
        /// </summary>
        public MainWindow()
        {
            _representation = new Representation();
            _score = new Score();
            _nextTetromino = new Tetromino(4, 0);
            
            InitializeComponent();
            Timer();
            
            NewTetromino();
            RenderGrid();
        }
        
        /// <summary>
        /// Clears the board otherwise for each movement a new tetromino will be displayed on top of
        /// the already existing one. Then Renders the tetromino.
        /// </summary>
        private void RenderGrid()
        {
            TetrisGrid.Children.Clear();
            RenderTetromino(_tetromino, TetrisGrid);
            RenderLandedTetrominos();
        }

        /// <summary>
        /// Sets the next tetromino as the current tetromino and than creates a new next tetromino and does
        /// the same with the matrices.
        /// Also sets the start position of the current tetromino. 
        /// </summary>
        /// <returns>True if the game is lost(new tetromino cant be put in an empty spot at the top, else false</returns>
        private bool NewTetromino()
        {
            NextGrid.Children.Clear();
            _tetromino = _nextTetromino;
            if (_representation.CheckCollision(_tetromino))
            {
                GameOver();
                return true;
            };
            _nextTetromino = new Tetromino(4, 0);
            RenderTetromino(_nextTetromino, NextGrid);
            return false;
        }

        /// <summary>
        /// Start a DispatcherTimer because those don't interupt the program
        /// This timer is used for determining the drop speed of tetrominoes.
        /// </summary>
        private void Timer()
        {
            _dpt = new DispatcherTimer();
            _dpt.Tick += dispatcherTimer_Tick;
            _dpt.Interval = new TimeSpan(0, 0, 0, 0, 700);
            _dpt.Start();
        }

        /// <summary>
        /// Starts a dispatcherTimer because those are non blocking.
        /// This timer is used to determine the speed at which tetromino's
        /// are falling 
        /// </summary>
        /// <param name="sender"></param> 
        /// <param name="e"></param>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (DropTetromino()) return;
            RenderGrid();
            DevelopmentInfo(); //TODO remove before release
        }

        /// <summary>
        /// Drops the tetromino every ... milliseconds
        /// Checks if the tetromino can drop without colliding with other tetrominos or the board bounds
        /// if it will collide with bounds or other tetromino's the tetromino will be put in the representation board
        /// and the representation checks if there are any full rows, if so they will be deleted
        /// if there are deleted rows score and level will be calculated again
        /// if the level is updated so will the gamespeed(the interval is reduced by 10% per level)
        /// lastly a new tetromino will be added and the board will be rendered again 
        /// </summary>
        private bool DropTetromino()
        {
            if (_representation.IsInRangeOfBoard(_tetromino, givenYOffset: 1) //if in range of the board
                && !_representation.CheckCollision(_tetromino,
                    givenYOffset: 1)) //if not collides with other tetromino's
                _tetromino.OffsetY++;
            else
            {
                _representation.PutTetrominoInBoard(_tetromino);
                var deletedRows = _representation.HandleRowDeletion();
                if (deletedRows != 0)
                {
                    _score.HandleScore(deletedRows);
                    if (_score.HandleLevel())
                        _dpt.Interval = new TimeSpan(0, 0, 0, 0, Convert.ToInt32(_dpt.Interval.Milliseconds * 0.9));
                }
                levelTextBox.Text = _score.Level.ToString();
                scoreTextBox.Text = _score.Points.ToString();
                linesTextBox.Text = _score.Rows.ToString();
                return NewTetromino();
            }
            return false;
        }

        /// <summary>
        /// Renders all tetrominos that are in the representation
        /// </summary>
        private void RenderLandedTetrominos()
        {
            for (var y = 0; y < _representation.Board.GetLength(0); y++)
            for (var x = 0; x < _representation.Board.GetLength(1); x++)
            {
                var block = _representation.Board[y, x];
                if (block == 0) continue; //block does not need to be rendered when it is 0 because its empty

                var rectangle = CreateRectangle(
                    ConvertNumberToBrush(_representation.Board[y, x])); // TODO Fix colors corresponding to tetromino
                TetrisGrid.Children.Add(rectangle);

                Grid.SetRow(rectangle, y);
                Grid.SetColumn(rectangle, x);
            }
        }

        /// <summary>
        /// Constructs the given tetromino by getting the int[,] from the matrix. For each cell that
        /// is not '1' it creates nothing because that should be empty. For every 1 a block will be drawn.
        /// Then creates a rectangle of the mapped tetromino and places it in the given grid (trough) param.
        /// </summary>
        /// <param name="matrixValue">int[,] from the matrix that belongs to the tetromino parameter</param>
        /// <param name="tetromino"></param>
        /// <param name="grid">TetrisGrid or NextGrid for next tetromino</param>
        private void RenderTetromino(Tetromino tetromino, Grid grid)
        {
            tetromino.CalculatePositions().ForEach(coordinate =>
            {
                var (y, x) = coordinate;
                var rectangle = CreateRectangle(Tetromino.DetermineColor(tetromino.Shape));
                grid.Children.Add(rectangle);

                Grid.SetRow(rectangle, y);
                Grid.SetColumn(rectangle, grid == TetrisGrid ? x : x - 4);
            });
        }

        private void GameOver()
        {
            _dpt.Stop();
            _paused = true;
            GameOverText.Text = "Game over";
        }


        /// <summary>
        /// Checks if a rotation would cross the border and if so, corrects the position accordingly.
        /// </summary>
        /// <summary>
        /// C# function that triggers when a key is pressed.
        /// This is how the user controls the game
        /// </summary>
        /// <param name="e">pressed key</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!_dpt.IsEnabled) return;

            switch (e.Key)
            {
                //move right
                case Key.Right when _representation.IsInRangeOfBoard(_tetromino, 1)
                                    && !_representation.CheckCollision(_tetromino, givenYOffset: 0, givenXOffset: 1):
                    _tetromino.OffsetX++;
                    break;
                //move left
                case Key.Left when _representation.IsInRangeOfBoard(_tetromino, -1)
                                   && !_representation.CheckCollision(_tetromino, givenYOffset: 0, givenXOffset: -1):
                    _tetromino.OffsetX--;
                    break;
                //Rotate clockwise
                case Key.Up:
                    _tetromino.Matrix = _tetromino.Matrix.Rotate90();
                    CorrectRotation();
                    break;
                //Rotate counter clockwise
                case Key.Down:
                    _tetromino.Matrix = _tetromino.Matrix.Rotate90CounterClockwise();
                    CorrectRotation();
                    break;
                //fully move down (hard drop)
                case Key.Space when _representation.IsInRangeOfBoard(_tetromino, 0, 1)
                                    && !_representation.CheckCollision(_tetromino, givenYOffset: 1):
                    _tetromino.OffsetY++;
                    OnKeyDown(e);
                    break;
                //move down by one (soft drop)
                case Key.LeftShift when _representation.IsInRangeOfBoard(_tetromino, 0, 1)
                                    && !_representation.CheckCollision(_tetromino, givenYOffset: 1):
                    _tetromino.OffsetY++;
                    break;
                //Only used in development
                case Key.E:
                    _tetromino.OffsetY--;
                    break;
                default:
                    return;
            }

            RenderGrid();
        }

        private void CorrectRotation()
        {
            if (_representation.IsInRangeOfBoard(_tetromino)) return; //return when check is not necessary 

            //left side of the board
            if (_tetromino.OffsetX < 0)
            {
                _tetromino.OffsetX = 0;
                return;
            }

            //right side of the board
            var xCoordinates = _tetromino.CalculatePositions().Select(coordinate => coordinate.Item2).ToList();
            _tetromino.OffsetX -= xCoordinates.Max() - _representation.Board.GetLength(1) + 1;
        }

        private void Quit(object sender, RoutedEventArgs routedEventArgs) => Application.Current.Shutdown();

        private void Pause(object sender, RoutedEventArgs routedEventArgs)
        {
            var button = (Button) sender;
            button.Content = (string) button.Content == "Pause" ? "Resume" : "Pause";
            _dpt.IsEnabled = !_dpt.IsEnabled;
        }

        private static Rectangle CreateRectangle(Brush color) => new()
        {
            Width = 30, // Width of a 'cell' in the Grid
            Height = 30, // Height of a 'cell' in the Grid
            Stroke = Brushes.Black, // Border
            StrokeThickness = 0.75, // Border thickness
            Fill = color // Background color
        };

        private static Brush ConvertNumberToBrush(int num)
        {
            return num switch
            {
                1 => Tetromino.DetermineColor(TetrominoShape.O),
                2 => Tetromino.DetermineColor(TetrominoShape.T),
                3 => Tetromino.DetermineColor(TetrominoShape.J),
                4 => Tetromino.DetermineColor(TetrominoShape.L),
                5 => Tetromino.DetermineColor(TetrominoShape.S),
                6 => Tetromino.DetermineColor(TetrominoShape.Z),
                7 => Tetromino.DetermineColor(TetrominoShape.I),
                _ => throw new ArgumentOutOfRangeException(nameof(num), num, null)
            };
        }

        // For debugging purposes
        private void DevelopmentInfo()
        {
            var i = 1;
            foreach (var cell in _representation.Board)
            {
                if (i % 10 == 0)
                {
                    Console.WriteLine(cell);
                }
                else Console.Write(cell);

                i++;
            }
        }
    }
}