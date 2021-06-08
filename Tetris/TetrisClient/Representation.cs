using System;
using System.Collections.Generic;

namespace TetrisClient
{
    public class Representation
    {
        public int[,] Board;
        
        public Representation() => Board = GenerateEmptyBoard();
        
        /// <summary>
        /// TODO: create implementation
        /// 3D array that represents the board.
        /// The numbers (1-7) indicate from which tetronimo it was.
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
        public bool IsInRangeOfBoard(Tetronimo tetronimo, int givenXOffset = 0, int givenYOffset = 0)
        {
            //check if any of the tetromino's blocks is out of bounds
            for (var y = 0; y < tetronimo.Matrix.Value.GetLength(0); y++) //dimension 0 = y
            for (var x = 0; x < tetronimo.Matrix.Value.GetLength(1); x++) //dimension 1 = x
            {
                //do nothing when cell in the tetromino matrix is 0(not a block)
                if(tetronimo.Matrix.Value[y,x] == 0) continue;
                
                var yWithOffset = y + tetronimo.OffsetY + givenYOffset;
                var xWithOffset = x + tetronimo.OffsetX + givenXOffset;
                
                if (yWithOffset > Board.GetLength(0) - 1) return false;
                if (xWithOffset > Board.GetLength(1) - 1 || xWithOffset < 0) return false;
            }
            return true;
        }

        public void PutTetrominoInBoard(Tetronimo tetronimo)
        {
            //render the tetromino
            //loop trough all blocks in the tetromino
            for (var y = 0; y < tetronimo.Matrix.Value.GetLength(0); y++) //dimension 0 = y
            for (var x = 0; x < tetronimo.Matrix.Value.GetLength(1); x++) //dimension 1 = x
            {
                //do nothing when cell in the tetromino matrix is 0(not a block)
                if(tetronimo.Matrix.Value[y,x] == 0) continue;

                //put the value at the correct spot
                Board[y + tetronimo.OffsetY, x + tetronimo.OffsetX] = 1; //TODO should be tetromino's corresponding number
            } 
        }
    }
}