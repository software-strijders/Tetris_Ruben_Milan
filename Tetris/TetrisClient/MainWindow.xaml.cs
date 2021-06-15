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
    public partial class MainWindow
    {
        private readonly TetrisEngine _engine = new();
        private DispatcherTimer _renderTimer;

        public MainWindow()
        {
            InitializeComponent();
            _engine.StartGame();
            Timer();
        }

        /// <summary>
        /// Start a DispatcherTimer because those don't interrupt the program
        /// This timer is only used for rendering, it matches the speed of the engine timer
        /// </summary>
        private void Timer()
        {
            _renderTimer = new DispatcherTimer();
            _renderTimer.Tick += dispatcherTimer_Tick;
            _renderTimer.Interval = _engine.GameTimer.Interval;
            _renderTimer.Start();
        }

        /// <summary>
        /// Starts a dispatcherTimer because those are non blocking.
        /// This timer is used to determine the speed at which tetromino's
        /// are falling 
        /// </summary>
        /// <param name="sender"></param> 
        /// <param name="e"></param>
        private void dispatcherTimer_Tick(object sender, EventArgs e) => UpdateGame();

        /// <summary>
        /// Renders all landed tetrominos, the falling tetromino and the next tetromino
        /// </summary>
        private void RenderGrid()
        {
            TetrisGrid.Children.Clear();

            RenderLandedTetrominos();

            RenderTetromino(_engine.Tetromino, TetrisGrid);
            RenderTetromino(_engine.CreateGhostTetromino(), TetrisGrid, 0.30);

            NextGrid.Children.Clear();
            RenderTetromino(_engine.NextTetromino, NextGrid);
        }

        /// <summary>
        /// Constructs the given tetromino by getting the int[,] from the matrix. For each cell that
        /// is '0' it creates nothing because that should be empty. For every number that is not 0 a block will be drawn.
        /// Then creates a rectangle of the mapped tetromino and places it in the given grid (trough) param.
        /// </summary>
        /// <param name="tetromino"></param>
        /// <param name="grid">TetrisGrid or NextGrid for next tetromino</param>
        /// <param name="opacity">Opacity, used for rendering a ghost tetromino</param>
        private void RenderTetromino(Tetromino tetromino, Panel grid, double opacity = 1)
        {
            tetromino.CalculatePositions().ForEach(coordinate =>
            {
                var (y, x) = coordinate;
                var rectangle = CreateRectangle(Tetromino.DetermineColor(tetromino.Shape), opacity);
                grid.Children.Add(rectangle);

                Grid.SetRow(rectangle, y);
                Grid.SetColumn(rectangle, grid == TetrisGrid ? x : x - 4);
            });
        }

        /// <summary>
        /// Renders all tetrominos that are in the representation
        /// </summary>
        private void RenderLandedTetrominos()
        {
            var board = _engine.Representation.Board;

            for (var y = 0; y < board.GetLength(0); y++)
            for (var x = 0; x < board.GetLength(1); x++)
            {
                var block = board[y, x];
                if (block == 0) continue; //block does not need to be rendered when it is 0 because its empty

                var rectangle = CreateRectangle(ConvertNumberToBrush(board[y, x]));
                TetrisGrid.Children.Add(rectangle);

                Grid.SetRow(rectangle, y);
                Grid.SetColumn(rectangle, x);
            }
        }

        /// <summary>
        /// Updates all game info
        /// Stops the timer when the game is lost, also shows the gameOverText
        /// Sets the interval of the _renderTimer to the same speed as the game engine speed
        /// Sets all score texts to their corresponding value
        /// Renders all tetromino's
        /// </summary>
        private void UpdateGame()
        {
            if (_engine.GameOver)
            {
                _renderTimer.IsEnabled = false;
                GameOverText.Visibility = Visibility.Visible;
                return;
            }

            _renderTimer.Interval = _engine.GameTimer.Interval;

            LevelTextBlock.Text = _engine.Score.Level.ToString();
            ScoreTextBlock.Text = _engine.Score.Points.ToString();
            LinesTextBlock.Text = _engine.Score.Rows.ToString();

            RenderGrid();
        }

        /// <summary>
        /// C# function that triggers when a key is pressed.
        /// This is how the user controls the game
        /// </summary>
        /// <param name="e">pressed key</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // Non in-game actions related keyboard controls
            switch (e.Key)
            {
                case Key.Enter:
                    Restart();
                    break;
                case Key.P:
                    TogglePause(null, null);
                    break;
                case Key.Escape:
                    Quit(null, null);
                    break;
            }

            if (!_renderTimer.IsEnabled) return;

            // In-game actions
            switch (e.Key)
            {
                case Key.Right:
                    _engine.MoveRight();
                    break;
                case Key.Left:
                    _engine.MoveLeft();
                    break;
                case Key.Up:
                    _engine.HandleRotation("UP");
                    break;
                case Key.Down:
                    _engine.HandleRotation("DOWN");
                    break;
                case Key.Space:
                    _engine.HardDrop();
                    break;
                case Key.LeftShift:
                    _engine.SoftDrop();
                    break;
                default:
                    return;
            }

            UpdateGame();
        }

        /// <summary>
        /// Stops the timer
        /// Restarts the game engine
        /// Hides the game over text
        /// Starts the timer again
        /// Updates all game info
        /// </summary>
        private void Restart()
        {
            _renderTimer.Stop();
            _engine.Restart();
            GameOverText.Visibility = Visibility.Hidden;
            Timer();
            UpdateGame();
        }

        /// <summary>
        /// Toggles the timer and pauseButton text, also toggles the engine's pause
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routedEventArgs"></param>
        private void TogglePause(object sender, RoutedEventArgs routedEventArgs)
        {
            PauseButton.Content = (string) PauseButton.Content == "Pause" ? "Resume" : "Pause";
            _renderTimer.IsEnabled = !_renderTimer.IsEnabled;
            _engine.TogglePause();
        }

        /// <summary>
        /// Quits the game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routedEventArgs"></param>
        private void Quit(object sender, RoutedEventArgs routedEventArgs) => Application.Current.Shutdown();

        /// <summary>
        /// Creates a rectangle and gives it the given <paramref name="color"/>
        /// </summary>
        /// <param name="color">Brush that corresponds with the current tetromino</param>
        /// <param name="opacity">Opacity of the rectangle, used for ghost tetromino</param>
        /// <returns>Rectangle with the given <paramref name="color"/></returns>
        private static Rectangle CreateRectangle(Brush color, double opacity = 1) => new()
        {
            Width = 30, // Width of a 'cell' in the Grid
            Height = 30, // Height of a 'cell' in the Grid
            Stroke = Brushes.Black, // Border
            StrokeThickness = 0.75, // Border thickness
            Fill = color, // Background color
            Opacity = opacity // Opacity
        };

        /// <summary>
        /// Based on the <paramref name="num"/> given, determines what color should be returned.
        /// </summary>
        /// <param name="num">number of the TetrominoShape</param>
        /// <returns>Brush color that corresponds with the given number</returns>
        /// <exception cref="ArgumentOutOfRangeException">If an invalid number has been passed</exception>
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
    }
}