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
        
        //this needs to go
        private List<int> _currentYPoints;

        /// <summary>
        /// Initializes the component and the timer, then creates the first next Tetromino
        /// so it can be used in the NewTetromino method.
        /// After that it renders the new next tetronimo and renders the board with the first tetromino.
        /// </summary>
        public MainWindow()
        {
            _nextTetromino = new Tetronimo(4, 0);
            _representation = new Representation();
            InitializeComponent();
            Timer();
            NewTetromino();
            RenderTetromino(_nextTetromino, NextGrid);
            Board();
        }

        /// <summary>
        /// Sets the next tetromino as the current tetromino and than creates a new next tetromino and does
        /// the same with the matrices.
        /// Also sets the start position of the current tetromino. 
        /// </summary>
        private void NewTetromino()
        {
            _tetronimo = _nextTetromino;
            _nextTetromino = new Tetronimo(0, 4);
            _tetronimo.OffsetY = 0;
            _tetronimo.OffsetX = 3;
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
            _currentYPoints = new List<int>(); // For debugging purposes
            for (var i = 0; i < tetronimo.Matrix.Value.GetLength(0); i++)
            for (var j = 0; j < tetronimo.Matrix.Value.GetLength(1); j++)
            {
                //  If the value doesn't equal one, it does't have to get drawn
                if (tetronimo.Matrix.Value[i, j] != 1) continue;

                var rectangle = new Rectangle
                {
                    Width = 25, // Width of a 'cell' in the Grid
                    Height = 25, // Height of a 'cell' in the Grid
                    Stroke = Brushes.Black, // Border
                    StrokeThickness = 0.75, // Border thickness
                    Fill = Tetronimo.DetermineColor(tetronimo.shape) // Background color
                };

                grid.Children.Add(rectangle); // Add the rectangle to the grid
                if (grid.Equals(TetrisGrid))
                {
                    Grid.SetRow(rectangle, i + tetronimo.OffsetY);
                    Grid.SetColumn(rectangle, j + tetronimo.OffsetX);
                    _currentYPoints.Add(j + tetronimo.OffsetX); // For debugging purposes
                }
                else
                {
                    Grid.SetRow(rectangle, i);
                    Grid.SetColumn(rectangle, j);
                }
            }
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
            _tetronimo.OffsetY++;
            DevelopmentInfo();
            Board();
        }


        // For debugging purposes
        private void DevelopmentInfo()
        {
            yText.Text = "Y: " + _tetronimo.OffsetY;
            xText.Text = "X: " + _tetronimo.OffsetX;
            var points = _currentYPoints.Aggregate("", (current, point) => current + (" " + point));
            yList.Text = points;
        }

        /// <summary>
        /// Clears the board otherwise for each movement a new tetronimo will be displayed on top of
        /// the already existing one. Then Renders the tetromino.
        /// </summary>
        private void Board()
        {
            TetrisGrid.Children.Clear();
            RenderTetromino(_tetronimo, TetrisGrid);
        }

        /// <summary>
        /// Checks if the tetromino can move in the given direction.
        /// </summary>
        /// <param name="direction">Key.Right or Key.Left to specify the direction</param>
        /// <returns></returns>
        private bool IsMoveAllowed(Key direction)
        {
            foreach (var point in _currentYPoints) // For debugging purposes
            {
                switch (direction)
                {
                    case Key.Right:
                        if (point > 8) return false;
                        break;
                    case Key.Left:
                        if (point < 1) return false;
                        break;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks if a rotation would cross the border and if so, corrects the position accordingly.
        /// </summary>
        private void CorrectRotation()
        {
            if (_tetronimo.OffsetX < 0)
            {
                _tetronimo.OffsetX = 0;
                return;
            }

            Board();
            while (_currentYPoints.Max() > 9)
            {
                _tetronimo.OffsetX--;
                Board();
            }
        }

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
                case Key.Right:
                    if (!IsMoveAllowed(Key.Right)) return;
                    _tetronimo.OffsetX++;
                    break;
                case Key.Left:
                    if (!IsMoveAllowed(Key.Left)) return;
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
                case Key.Space:
                    _tetronimo.OffsetY++;
                    break;
                //Easter egg, unknown to the human race, used in development
                case Key.E:
                    _tetronimo.OffsetY--;
                    break;
            }

            Board();
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
    }
}