using System;
using System.Collections.Generic;
using static System.Linq.Enumerable;

namespace TetrisClient
{
    public class Representation
    {
        public int[,] Board;

        public Representation() => Board = GenerateEmptyBoard();

        /// <summary>
        /// multidimensional array that represents the board.
        /// The numbers (1-7) indicate from which tetromino it was.
        /// This way we can accurately keep control of the colors.
        /// </summary>
        /// <returns>board representation given in a multidimensional array</returns>
        private static int[,] GenerateEmptyBoard()
        {
            return new[,]
            {
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
            };
        }

        //GivenOffsets are used to think one step ahead, to calculate if the next move if possible.
        public bool IsInRangeOfBoard(Tetromino tetromino, int givenXOffset = 0, int givenYOffset = 0)
        {
            //check if any of the tetromino's blocks is out of bounds
            for (var y = 0; y < tetromino.Matrix.Value.GetLength(0); y++) //dimension 0 = y
            for (var x = 0; x < tetromino.Matrix.Value.GetLength(1); x++) //dimension 1 = x
            {
                //do nothing when cell in the tetromino matrix is 0(not a block)
                if (tetromino.Matrix.Value[y, x] == 0) continue;

                var yWithOffset = y + tetromino.OffsetY + givenYOffset;
                var xWithOffset = x + tetromino.OffsetX + givenXOffset;

                if (yWithOffset > Board.GetLength(0) - 1)
                    return false; //false if current block in loop is outside the board vertically
                if (xWithOffset > Board.GetLength(1) - 1 || xWithOffset < 0) return false; //^same but horizontally
            }

            return true;
        }

        //checks if any blocks of the given tetromino is the same as any of the occupied blocks in the board 
        //thinks one step ahead with the givenOffsets
        public bool CheckCollision(Tetromino tetromino, int givenXOffset = 0, int givenYOffset = 0)
        {
            var collided = false;
            for (var y = 0; y < Board.GetLength(0); y++) //dimension 0 = y
            for (var x = 0; x < Board.GetLength(1); x++) //dimension 1 = x
            {
                if (Board[y, x] != 0) //if block is not empty
                    tetromino.CalculatePositions().ForEach(coordinate =>
                    {
                        var (tetrominoY, tetrominoX) = coordinate;
                        if (tetrominoY == y - givenYOffset && tetrominoX == x - givenXOffset)
                        {
                            collided = true; //TODO should simply be return true but that does not work for unknown reasons
                        }
                    });
            }

            return collided;
        }

        public void PutTetrominoInBoard(Tetromino tetromino)
        {
            //render the tetromino
            //loop trough all blocks in the tetromino
            for (var y = 0; y < tetromino.Matrix.Value.GetLength(0); y++) //dimension 0 = y
            for (var x = 0; x < tetromino.Matrix.Value.GetLength(1); x++) //dimension 1 = x
            {
                //do nothing when cell in the tetromino matrix is 0(not a block)
                if (tetromino.Matrix.Value[y, x] == 0) continue;

                //put the value at the correct spot
                Board[y + tetromino.OffsetY, x + tetromino.OffsetX]
                    = ConvertTetrominoShapeToNumber(tetromino.Shape);
            }
        }

        /// <summary>
        /// General method that's called after each tick.
        /// Evaluates if rows are full and handles it.
        /// </summary>
        public int HandleRowDeletion()
        {
            var fullRows = FullRows();
            return DeleteFullRows(fullRows);
        }

        /// <summary>
        /// Checks if there are any rows that are full (x axis)
        /// </summary>
        /// <returns>row numbers that are full</returns>
        private List<int> FullRows()
        {
            var fullRows = new List<int>();
            for (var yAxis = 0; yAxis < Board.GetLength(0); yAxis++)
                if (Range(0, Board.GetLength(1)).Select(x => Board[yAxis, x]).ToList().FindAll(x => x > 0).Count == 10)
                    fullRows.Add(yAxis);
            return fullRows;
        }

        /// <summary>
        /// Deletes the rows that are full.
        /// </summary>
        /// <param name="fullRows"></param>
        private int DeleteFullRows(List<int> fullRows)
        {
            var rowsDeleted = 0;
            for (var y = 0; y < Board.GetLength(0); y++)
            {
                if (!fullRows.Contains(y)) continue;
                for (var x = 0; x < Board.GetLength(1); x++)
                    Board[y, x] = 0;

                DropFloatingTetrominos(y);
                rowsDeleted++;
                fullRows.Remove(y);
            }

            Console.WriteLine(rowsDeleted);
            return rowsDeleted;
        }

        /// <summary>
        /// Looks at the rows above the deleted row and copies them at the row below.
        /// </summary>
        /// <param name="deletedRow"></param>
        private void DropFloatingTetrominos(int deletedRow)
        {
            for (var y = deletedRow; y > 0; y--) //dimension 0 = y
            for (var x = 0; x < Board.GetLength(1); x++) //dimension 1 = x
                Board[y, x] = Board[y - 1, x];
        }

        /// <summary>
        /// Converts the shape of the tetromino to its corresponding number
        /// this number will later be used in the UI to match it's corresponding color(Brush)
        /// </summary>
        /// <param name="tetrominoShape"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static int ConvertTetrominoShapeToNumber(TetrominoShape tetrominoShape)
        {
            return tetrominoShape switch
            {
                TetrominoShape.O => 1,
                TetrominoShape.T => 2,
                TetrominoShape.J => 3,
                TetrominoShape.L => 4,
                TetrominoShape.S => 5,
                TetrominoShape.Z => 6,
                TetrominoShape.I => 7,
                _ => throw new ArgumentOutOfRangeException(nameof(tetrominoShape), tetrominoShape, null)
            };
        }
    }
}