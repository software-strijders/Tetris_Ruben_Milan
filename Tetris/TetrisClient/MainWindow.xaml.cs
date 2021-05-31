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

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            _offsetY++;
            Board();
        }

        private void Board()
        {
            TetrisGrid.Children.Clear();
            // Get board representation of landed bricks.
            var values = _matrix.Value;
            for (var i = 0; i < values.GetLength(0); i++)
            {
                for (var j = 0; j < values.GetLength(1); j++)
                {
                    // Als de waarde niet gelijk is aan 1,
                    // dan hoeft die niet getekent te worden:
                    if (values[i, j] != 1) continue;

                    var rectangle = new Rectangle
                    {
                        Width = 25, // Breedte van een 'cell' in de Grid
                        Height = 25, // Hoogte van een 'cell' in de Grid
                        Stroke = Brushes.Black, // De rand
                        StrokeThickness = 0.75, // Dikte van de rand
                        Fill = Tetronimo.DetermineColor(_tetronimo.shape), // Achtergrondkleur
                    };

                    TetrisGrid.Children.Add(rectangle); // Voeg de rectangle toe aan de Grid
                    Grid.SetRow(rectangle, i + _offsetY); // Zet de rij
                    Grid.SetColumn(rectangle, j + _offsetX); // Zet de kolom
                }
            }
        }

        private bool IsMoveAllowed(Key direction)
        {
            if (direction.Equals(Key.Right))
                return _offsetX <= 10;
            return _offsetX >= 1;
        }

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

            Console.WriteLine(_offsetX + " " + _offsetY);
            Board();
        }
    }
}