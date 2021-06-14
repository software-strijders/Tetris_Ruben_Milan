using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TetrisClient
{
    public class TetrisEngine
    {
        private MainWindow _mainWindow;

        private Representation _representation;
        private Tetromino _tetromino;
        private Tetromino _nextTetromino;
        private Score _score;
        private DispatcherTimer _dpt;

        public void StartGame()
        {
            _representation = new Representation();
            _score = new Score();
            _nextTetromino = new Tetromino(4, 0);
            Timer();
            NewTetromino();
            RenderGrid();
        }

        /// <summary>
        /// Sets the next tetromino as the current tetromino and than creates a new next tetromino and does
        /// the same with the matrices.
        /// Also sets the start position of the current tetromino. 
        /// </summary>
        /// <returns>True if the game is lost(new tetromino cant be put in an empty spot at the top, else false</returns>
        public bool NewTetromino()
        {
            _mainWindow.NextGrid.Children.Clear();
            _tetromino = _nextTetromino;
            
            if (_representation.CheckCollision(_tetromino))
            {
                GameOver();
                return true;
            }
            
            _nextTetromino = new Tetromino(4, 0);
            RenderTetromino(_nextTetromino, _mainWindow.NextGrid);
            return false;
        }
        
        /// <summary>
        /// Constructs the given tetromino by getting the int[,] from the matrix. For each cell that
        /// is not '1' it creates nothing because that should be empty. For every 1 a block will be drawn.
        /// Then creates a rectangle of the mapped tetromino and places it in the given grid (trough) param.
        /// </summary>
        /// <param name="tetromino"></param>
        /// <param name="grid">TetrisGrid or NextGrid for next tetromino</param>
        public void RenderTetromino(Tetromino tetromino, Panel grid)
        {
            tetromino.CalculatePositions().ForEach(coordinate =>
            {
                var (y, x) = coordinate;
                var rectangle = CreateRectangle(Tetromino.DetermineColor(tetromino.Shape));
                grid.Children.Add(rectangle);

                Grid.SetRow(rectangle, y);
                Grid.SetColumn(rectangle, grid == _mainWindow.TetrisGrid ? x : x - 4);
            });
            RenderGhostTetromino();
        }
        
        /// <summary>
        /// Renders a ghost tetromino, tetromino is copied from _tetromino
        /// and then rendered on the bottom of the playfield with a lower opacity
        /// </summary>
        private void RenderGhostTetromino()
        {
            var ghostTetromino = new Tetromino(_tetromino.OffsetX,
                _tetromino.OffsetY,
                _tetromino.Matrix,
                _tetromino.Shape);
            while (_representation.IsInRangeOfBoard(ghostTetromino, 0, 1)
                   && !_representation.CheckCollision(ghostTetromino, givenYOffset: 1))
                ghostTetromino.OffsetY++;
            
            ghostTetromino.CalculatePositions().ForEach(coordinate => {
                var (y, x) = coordinate;
                var rectangle = CreateRectangle(Tetromino.DetermineColor(_tetromino.Shape), 0.20);
                _mainWindow.TetrisGrid.Children.Add(rectangle);

                Grid.SetRow(rectangle, y);
                Grid.SetColumn(rectangle, x);
            });
        }
        
        /// <summary>
        /// Clears the board otherwise for each movement a new tetromino will be displayed on top of
        /// the already existing one. Then Renders the tetromino.
        /// </summary>
        public void RenderGrid()
        {
            _mainWindow.TetrisGrid.Children.Clear();
            RenderTetromino(_tetromino, _mainWindow.TetrisGrid);
            RenderLandedTetrominos();
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
                _mainWindow.TetrisGrid.Children.Add(rectangle);

                Grid.SetRow(rectangle, y);
                Grid.SetColumn(rectangle, x);
            }
        }

        public void MoveRight()
        {
            if (MovePossible(offsetInBoardX: 1, offsetCollisionX: 1))
            {
                _tetromino.OffsetX++;
            }
        }

        public void MoveLeft()
        {
            if (MovePossible(offsetInBoardY: -1, offsetCollisionY: -1))
            {
                _tetromino.OffsetX--;
            }
        }

        public void HandleRotation(string type)
        {
            if (type != "UP" && type != "DOWN") return;
            
            var offsetsToTest = new[] {0,1,-1,2,-2};
            foreach (var offset in offsetsToTest)
            {
                if (_representation.CheckTurnCollision(_tetromino, type, offset)) continue;
                _tetromino.OffsetX += offset;
                _tetromino.Matrix = type switch
                {
                    "UP" => _tetromino.Matrix.Rotate90(),
                    "DOWN" => _tetromino.Matrix.Rotate90CounterClockwise(),
                    _ => _tetromino.Matrix
                };
                break;
            }
        }

        public void HardDrop()
        {
            var movePossible = true;
            while (movePossible)
            {
                movePossible = SoftDrop();
            }
        }

        public bool SoftDrop()
        {
            if (!MovePossible(offsetInBoardX: 0, offsetInBoardY: 1, offsetCollisionY: 1)) return false;
            _tetromino.OffsetY--;
            return true;
        }


        private bool MovePossible(int offsetInBoardX = 0, int offsetInBoardY = 0, int offsetCollisionX = 0,
            int offsetCollisionY = 0)
        {
            return _representation.IsInRangeOfBoard(_tetromino, offsetInBoardX, offsetInBoardY)
                   && !_representation.CheckCollision(_tetromino, offsetCollisionX, offsetCollisionY);
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
        }
        
        /// <summary>
        /// Drops the tetromino every ... milliseconds
        /// Checks if the tetromino can drop without colliding with other tetrominos or the board bounds
        /// if it will collide with bounds or other tetromino's the tetromino will be put in the representation board
        /// and the representation checks if there are any full rows, if so they will be deleted
        /// if there are deleted rows score and level will be calculated again
        /// if the level is updated so will the game speed (the interval is reduced by 10% per level)
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
                if (deletedRows == 0) return NewTetromino();
                _score.HandleScore(deletedRows);
                if (_score.HandleLevel())
                    _dpt.Interval = new TimeSpan(0, 0, 0, 0, Convert.ToInt32(_dpt.Interval.Milliseconds * 0.9));
                UpdateTextBoxes();
                return NewTetromino();
            }
            return false;
        }

        private void GameOver()
        {
            _dpt.Stop();
            _mainWindow.gameOverText.Visibility = Visibility.Visible;
        }
        
        public void Restart()
        {
            _representation = new Representation();
            _score = new Score();
            _nextTetromino = new Tetromino(4, 0);

            StartGame();
            _dpt.Stop();
            Timer();
            UpdateTextBoxes();
            _mainWindow.gameOverText.Visibility = Visibility.Hidden;
            NewTetromino();
            
            RenderGrid();
        }

        public void Pause()
        {
            _dpt.IsEnabled = !_dpt.IsEnabled;
        }

        /// <summary>
        /// Updates the level-, score-, and linesTextBox with the updated values.
        /// </summary>
        private void UpdateTextBoxes()
        {
            _mainWindow.levelTextBlock.Text = _score.Level.ToString();
            _mainWindow.scoreTextBlock.Text = _score.Points.ToString();
            _mainWindow.linesTextBlock.Text = _score.Rows.ToString();
        }

        /// <summary>
        /// Creates a rectangle and gives it the given <paramref name="color"/>
        /// </summary>
        /// <param name="color">Brush that corresponds with the current tetromino</param>
        /// <param name="opacity"></param>
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