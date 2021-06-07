using System;

namespace TetrisClient
{
    public class Representation
    {
        public int[,] Landed { get; private set; }
        
        public Representation() => Landed = GenerateEmptyBoard();
        
        /// <summary>
        /// TODO: create implementation
        /// 3D array that represents the board.
        /// The numbers (1-7) indicate from which tetronimo it was.
        /// This way we can accurately keep control of the colors.
        /// </summary>
        /// <returns>board representation given in a 3D array</returns>
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

        public void PlaceTetrominoInBoard(Tetronimo tetronimo)
        {
            for (var i = 0; i < tetronimo.Matrix.Value.Length; i++)
            {
                       
            }
        }
    }
}