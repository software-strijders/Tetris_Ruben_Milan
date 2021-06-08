using System;
using System.Collections.Generic;
using System.Linq;
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
        
        private Tetronimo _tetronimo;
        private Tetronimo _nextTetromino;
        private DispatcherTimer _dpt;
        private TimeSpan _tickInterval = new(0, 0, 0, 0, 700);
        private bool _paused; // Default value is false

        /// <summary>
        /// Initializes the component and the timer, then creates the first next Tetromino
        /// so it can be used in the NewTetromino method.
        /// After that it renders the new next tetronimo and renders the board with the first tetromino.
        /// </summary>
        public MainWindow()
        {
            _representation = new Representation();
            
            InitializeComponent();
            Timer();
            
            _nextTetromino = new Tetronimo(3, 0);
            NewTetromino();

            Board();
        }
        
        /// <summary>
        /// Clears the board otherwise for each movement a new tetronimo will be displayed on top of
        /// the already existing one. Then Renders the tetromino.
        /// </summary>
        private void Board()
        {
            TetrisGrid.Children.Clear();
            RenderTetromino(_tetronimo, TetrisGrid);
            RenderBoard();
        }

        /// <summary>
        /// Sets the next tetromino as the current tetromino and than creates a new next tetromino and does
        /// the same with the matrices.
        /// Also sets the start position of the current tetromino. 
        /// </summary>
        private void NewTetromino()
        {
            NextGrid.Children.Clear();
            _tetronimo = _nextTetromino;
            _nextTetromino = new Tetronimo(4, 0);
            RenderTetromino(_nextTetromino, NextGrid);
        }
        
        /// <summary>
        /// Start a DispatcherTimer because those don't interupt the program
        /// This timer is used for determining the drop speed of tetrominoes.
        /// </summary>
        private void Timer()
        {
            _dpt = new DispatcherTimer();
            _dpt.Tick += dispatcherTimer_Tick;
            _dpt.Interval = _tickInterval;
            _dpt.Start();
        }
        
        /// <summary>
        /// Starts a dispatcherTimer because those are non blocking.
        /// This timer is used to determine the speed at which tetronimo's
        /// are falling 
        /// </summary>
        /// <param name="sender"></param> 
        /// <param name="e"></param>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            DropTetromino();
            Board();
            DevelopmentInfo();
        }

        private void DropTetromino()
        {
            // Named argument ------------------------------>|__________|
            if (_representation.IsInRangeOfBoard(_tetronimo, givenYOffset: 1) /* || collides with other Tetromino? */)
                _tetronimo.OffsetY++;
            else
            {
                _representation.PutTetrominoInBoard(_tetronimo);
                NewTetromino();
                Board();
            }
        }

        private void RenderBoard()
        {
            for (var y = 0; y < _representation.Board.GetLength(0); y++)
            for (var x = 0; x < _representation.Board.GetLength(1); x++)
            {
                if(_representation.Board[y,x] == 0) continue; //block does not need to be rendered when it is 0 because its empty
                
                var rectangle = CreateRectangle(Brushes.White); // TODO Fix colors corresponding to tetromino
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
        /// <param name="tetronimo"></param>
        /// <param name="grid">TetrisGrid or NextGrid for next tetromino</param>
        private void RenderTetromino(Tetronimo tetronimo, Grid grid)
        {
            tetronimo.CalculatePositions().ForEach(coordinate => {
                var (y, x) = coordinate;
                var rectangle = CreateRectangle(Tetronimo.DetermineColor(tetronimo.shape));
                grid.Children.Add(rectangle);

                Grid.SetRow(rectangle, y);
                Grid.SetColumn(rectangle, grid == TetrisGrid ? x : x - 4);
            });
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
            if (_paused)
                return;

            switch (e.Key)
            {
                case Key.Right when _representation.IsInRangeOfBoard(_tetronimo, 1):
                    _tetronimo.OffsetX++;
                    break;
                case Key.Left when _representation.IsInRangeOfBoard(_tetronimo, -1): 
                    _tetronimo.OffsetX--;
                    break;
                //Rotate clockwise
                case Key.Up:
                    _tetronimo.Matrix = _tetronimo.Matrix.Rotate90();
                    CorrectRotation();
                    break;
                //Rotate counter clockwise
                case Key.Down:
                    _tetronimo.Matrix = _tetronimo.Matrix.Rotate90CounterClockwise();
                    CorrectRotation();
                    break;
                //ToDo: instantly move down, we need to implement collision detection first before we can do this
                case Key.Space when _representation.IsInRangeOfBoard(_tetronimo, 0, 1):
                    _tetronimo.OffsetY++;
                    break;
                //Only used in development
                case Key.E:
                    _tetronimo.OffsetY--;
                    break;
            }
            DevelopmentInfo(); //TODO remove
            Board();
        }

        private void CorrectRotation()
        {
            if (_representation.IsInRangeOfBoard(_tetronimo)) return; //return when check is not necessary 
            
            //left side
            if (_tetronimo.OffsetX < 0)
            {
                _tetronimo.OffsetX = 0;
                return;
            }

            //right side
            var xCoordinates = _tetronimo.CalculatePositions().Select(coordinate => coordinate.Item2).ToList();
            _tetronimo.OffsetX -= xCoordinates.Max() - _representation.Board.GetLength(1)+1 ;
        }

        private void Quit(object sender, RoutedEventArgs routedEventArgs) => Application.Current.Shutdown();

        private void Pause(object sender, RoutedEventArgs routedEventArgs)
        {
            _paused = !_paused;

            var button = (Button) sender;
            button.Content = ReferenceEquals(button.Content, "Pause") ? "Resume" : "Pause";

            if (_paused) _dpt.Stop();
            else _dpt.Start();
        }
        
        private static Rectangle CreateRectangle(Brush color) => new()
            {
                Width = 25, // Width of a 'cell' in the Grid
                Height = 25, // Height of a 'cell' in the Grid
                Stroke = Brushes.Black, // Border
                StrokeThickness = 0.75, // Border thickness
                Fill = color // Background color
            };
        
        // For debugging purposes
        private void DevelopmentInfo()
        {
            yText.Text = "Y: " + _tetronimo.OffsetY;
            xText.Text = "X: " + _tetronimo.OffsetX;

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