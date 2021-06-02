using System;
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
    public partial class MainWindow : Window
    {
        private int _offsetX;
        private int _offsetY;
        private Matrix _matrix;
        private Tetronimo _tetronimo;
        private TimeSpan _tickInterval = new(0, 0, 1);

        public MainWindow()
        {
            InitializeComponent();
            Timer();

            _tetronimo = new Tetronimo(0, 4);
            _matrix = new Matrix(_tetronimo.IntArray);
            _matrix = _matrix.Rotate90();
            _offsetY = 0;
            _offsetX = 3;

            Board();
        }

        private void Timer()
        {
            var dpt = new DispatcherTimer();
            dpt.Tick += new EventHandler(dispatcherTimer_Tick);
            dpt.Interval = _tickInterval;

            dpt.Start();
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
            _offsetY++;
            Board();
        }

        private void Board()
        {
            // TODO: Get board representation of landed bricks.
            // Clear the board otherwise for each movement a new tetronimo will be displayed on top of
            // the already existing one.
            TetrisGrid.Children.Clear();
            var values = _matrix.Value;
            for (var i = 0; i < values.GetLength(0); i++)
            for (var j = 0; j < values.GetLength(1); j++)
            {
                //  If the value doesn't equal one, it doens't have to get drawn
                if (values[i, j] != 1) continue;

                var rectangle = new Rectangle
                {
                    Width = 25,                                         // Width of a 'cell' in the Grid
                    Height = 25,                                        // Height of a 'cell' in the Grid
                    Stroke = Brushes.Black,                             // Border
                    StrokeThickness = 0.75,                             // Border thickness
                    Fill = Tetronimo.DetermineColor(_tetronimo.shape)   // Background color
                };

                TetrisGrid.Children.Add(rectangle);                     // Add the rectangle to the grid
                Grid.SetRow(rectangle, i + _offsetY);
                Grid.SetColumn(rectangle, j + _offsetX);
            }
        }

        //TODO: improve
        private bool IsMoveAllowed(Key direction)
        {
            if (direction.Equals(Key.Right))
                return _offsetX <= 10;
            return _offsetX >= 1;
        }
        
        /// <summary>
        /// C# function that triggers when a key is pressed.
        /// This is how the user will control the game
        /// </summary>
        /// <param name="e">pressed key</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Right:
                    if (!IsMoveAllowed(Key.Right)) break;
                    _offsetX++;
                    break;
                case Key.Left:
                    if (!IsMoveAllowed(Key.Left)) break;
                    _offsetX--;
                    break;
                //Rotate clockwise
                case Key.Up:
                    _matrix = _matrix.Rotate90();
                    break;
                //Rotate counter clockwise
                case Key.Down:
                    _matrix = _matrix.Rotate90CounterClockwise();
                    break;
                //ToDo: instantly move down, we need to implement collision detection first before we can do this
                case Key.Space:
                    _offsetY++;
                    break;
                //Easter egg, unknown to the human race
                case Key.E:
                    _offsetY--;
                    break;
            }

            Board();
        }
    }
}