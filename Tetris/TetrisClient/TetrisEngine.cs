using System;
using System.Windows.Threading;

namespace TetrisClient
{
    public class TetrisEngine
    {
        public Representation Representation;
        public Tetromino Tetromino;
        public Tetromino NextTetromino;
        public Score Score;
        public DispatcherTimer GameTimer;
        public bool GameOver;
        private Random _random;

        /// <summary>
        /// Starts the game, creates all items
        /// Starts the timer
        /// </summary>
        public void StartGame(int? seed = null)
        {
            if (seed != null) _random = new Random((int) seed);
            GameOver = false;
            Representation = new Representation();
            Score = new Score();
            NextTetromino = _random == null ? new Tetromino(4, 0) : new Tetromino(4, 0, GenerateShape());
            Timer();
            NewTetromino();
        }

        /// <summary>
        /// Picks a random Tetromino Shape.
        /// </summary>
        /// <returns>TetrominoShape enum</returns>
        private TetrominoShape GenerateShape()
        {
            var values = Enum.GetValues(typeof(TetrominoShape));
            return (TetrominoShape) values.GetValue(_random.Next(values.Length));
        }

        /// <summary>
        /// Start a DispatcherTimer because those don't interrupt the program
        /// This timer is used for determining the drop speed of tetrominos.
        /// </summary>
        private void Timer()
        {
            GameTimer = new DispatcherTimer();
            GameTimer.Tick += dispatcherTimer_Tick;
            GameTimer.Interval = new TimeSpan(0, 0, 0, 0, 700);
            GameTimer.Start();
        }

        /// <summary>
        /// Every tick of the timer it will drop the tetromino and handle the score
        /// If a level is upped the speed reduced by 10%
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (DropTetromino()) return;
            if (HandleScore())
                GameTimer.Interval = new TimeSpan(0, 0, 0, 0, Convert.ToInt32(GameTimer.Interval.Milliseconds * 0.9));
            NewTetromino();
        }

        /// <summary>
        /// Drops the tetromino every <c>GameTimer.Interval</c> milliseconds
        /// Checks if the move is possible
        /// if that is not true the tetromino will be mounted in the representation
        /// </summary>
        /// <returns>true if drop was possible else false</returns>
        private bool DropTetromino()
        {
            if (MovePossible(offsetInBoardY: 1, offsetCollisionY: 1))
            {
                Tetromino.OffsetY++;
                return true;
            }

            Representation.PutTetrominoInBoard(Tetromino);
            return false;
        }

        /// <summary>
        /// Sets the next tetromino as the current tetromino and than creates a new next tetromino and does
        /// the same with the matrices.
        /// Also sets the start position of the current tetromino. 
        /// </summary>
        /// <returns>True if the game is lost(new tetromino cant be put in an empty spot at the top, else false</returns>
        private void NewTetromino()
        {
            Tetromino = NextTetromino;
            if (Representation.CheckCollision(Tetromino))
            {
                GameTimer.IsEnabled = false;
                GameOver = true;
            }

            NextTetromino = _random == null ? new Tetromino(4, 0) : new Tetromino(4, 0, GenerateShape());
        }

        /// <summary>
        /// Creates a ghost tetromino, the ghost is copied from the current tetromino
        /// It gets dropped to as low as possible(kinda like the hard drop)
        /// </summary>
        /// <returns>The ghost tetromino</returns>
        public Tetromino CreateGhostTetromino()
        {
            var ghostTetromino = new Tetromino(Tetromino.OffsetX,
                Tetromino.OffsetY,
                Tetromino.Matrix,
                Tetromino.Shape);
            while (Representation.IsInRangeOfBoard(ghostTetromino, 0, 1)
                   && !Representation.CheckCollision(ghostTetromino, givenYOffset: 1))
                ghostTetromino.OffsetY++;

            return ghostTetromino;
        }

        //Moves the tetromino to the right if allowed
        public void MoveRight()
        {
            if (MovePossible(offsetInBoardX: 1, offsetCollisionX: 1))
                Tetromino.OffsetX++;
        }

        //Moves the tetromino to the left if allowed
        public void MoveLeft()
        {
            if (MovePossible(offsetInBoardX: -1, offsetCollisionX: -1))
                Tetromino.OffsetX--;
        }

        /// <summary>
        /// Tries to rotate a tetromino with given offsets, if one of them succeeds
        /// the tetromino will turn.
        /// </summary>
        /// <param name="type"> UP(clockwise) or DOWN(CounterClockWise)</param>
        public void HandleRotation(string type)
        {
            if (type is not "UP" and not "DOWN") return;

            var offsetsToTest = new[] {0, 1, -1, 2, -2};
            foreach (var offset in offsetsToTest)
            {
                if (Representation.CheckTurnCollision(Tetromino, type, offset)) continue;
                Tetromino.OffsetX += offset;
                Tetromino.Matrix = type switch
                {
                    "UP" => Tetromino.Matrix.Rotate90(),
                    "DOWN" => Tetromino.Matrix.Rotate90CounterClockwise(),
                    _ => Tetromino.Matrix
                };
                break;
            }
        }

        //Drops the current tetromino to as low as possible
        public void HardDrop()
        {
            while (SoftDrop())
            {
            }
        }

        //Drops the current tetromino by one
        public bool SoftDrop()
        {
            if (!MovePossible(offsetInBoardX: 0, offsetInBoardY: 1, offsetCollisionY: 1)) return false;
            Tetromino.OffsetY++;
            return true;
        }


        /// <summary>
        /// Checks if the move is possible, the move can be simulated by giving offsets
        /// </summary>
        /// <param name="offsetInBoardX">Checks for the game borders</param>
        /// <param name="offsetInBoardY">^</param>
        /// <param name="offsetCollisionX">Checks for a collision</param>
        /// <param name="offsetCollisionY">^</param>
        /// <returns></returns>
        private bool MovePossible(int offsetInBoardX = 0, int offsetInBoardY = 0, int offsetCollisionX = 0,
            int offsetCollisionY = 0)
        {
            return Representation.IsInRangeOfBoard(Tetromino, offsetInBoardX, offsetInBoardY)
                   && !Representation.CheckCollision(Tetromino, offsetCollisionX, offsetCollisionY);
        }

        /// <summary>
        /// Checks if there are any deleted rows, if so the score level will be recalculated.
        /// </summary>
        /// <returns>True if the level has changed</returns>
        private bool HandleScore()
        {
            var deletedRows = Representation.HandleRowDeletion();
            if (deletedRows == 0) return false;
            Score.HandleScore(deletedRows);
            return Score.HandleLevel();
        }

        /// <summary>
        /// Stops the timer
        /// Restarts the game
        /// Asks for an extra NewTetromino so that it will not use the one from last game
        /// </summary>
        public void Restart()
        {
            GameTimer.Stop();
            StartGame();
            NewTetromino();
        }

        /// <summary>
        /// toggle the timer pause
        /// </summary>
        public void TogglePause() => GameTimer.IsEnabled = !GameTimer.IsEnabled;
    }
}