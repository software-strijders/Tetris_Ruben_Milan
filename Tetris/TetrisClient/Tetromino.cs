using System;
using System.Collections.Generic;
using System.Windows.Media;
using static TetrisClient.TetrominoShape;

namespace TetrisClient
{
    /// <summary>
    /// Enum that represents the different kinds of tetronimo's.
    /// </summary>
    public enum TetrominoShape
    {
        O,
        T,
        J,
        L,
        S,
        Z,
        I,
    }

    public class Tetromino
    {
        public TetrominoShape Shape { get; private set; }
        public Matrix Matrix { get; set; }
        public int OffsetX;
        public int OffsetY;

        /// <summary>
        /// Default start position is at the left top (0,0).
        /// Constructor overloading is used if alternate spawnpoints
        /// are being chosen.
        /// </summary>
        public Tetromino() => new Tetromino(0, 0);

        public Tetromino(int offsetX, int offsetY)
        {
            var generatedShape = GenerateShape();
            Shape = generatedShape;
            Matrix = CreateShape(generatedShape);
            OffsetX = offsetX;
            OffsetY = offsetY;
        }

        /// <summary>
        /// Calculates all x and y positions from the tetromino in the board(also uses the offsets)
        /// </summary>
        /// <returns>All coordinates</returns>
        public List<(int, int)> CalculatePositions()
        {
            var coordinates = new List<(int, int)>();
            for (var y = 0; y < Matrix.Value.GetLength(0); y++)
            for (var x = 0; x < Matrix.Value.GetLength(1); x++)
            {
                if (Matrix.Value[y, x] == 0)
                    continue; //block does not need to be rendered when it is 0 because its empty
                coordinates.Add((y + OffsetY, x + OffsetX));
            }

            return coordinates;
        }

        /// <summary>
        /// Picks a random tetronimo.
        /// </summary>
        /// <returns>TetrominoShape enum</returns>
        private static TetrominoShape GenerateShape()
        {
            var values = Enum.GetValues(typeof(TetrominoShape));
            return (TetrominoShape) values.GetValue(new Random().Next(values.Length));
        }

        /// <summary>
        /// Gives back the 3D array that corresponds with the given tetronimo shape enum.
        /// </summary>
        /// <param name="shape">TetrominoShape enum</param>
        /// <returns>3D array that represents a tetronimo of the passed enum</returns>
        /// <exception cref="ArgumentOutOfRangeException">when an invalid entry is passed</exception>
        private static Matrix CreateShape(TetrominoShape shape) => shape switch
        {
            O => new Matrix(new[,] {{1, 1}, {1, 1}}),
            T => new Matrix(new[,] {{1, 1, 1}, {0, 1, 0}, {0, 0, 0}}),
            J => new Matrix(new[,] {{0, 1, 0}, {0, 1, 0}, {1, 1, 0}}),
            L => new Matrix(new[,] {{0, 1, 0}, {0, 1, 0}, {0, 1, 1}}),
            S => new Matrix(new[,] {{0, 1, 1}, {1, 1, 0}, {0, 0, 0}}),
            Z => new Matrix(new[,] {{1, 1, 0}, {0, 1, 1}, {0, 0, 0}}),
            I => new Matrix(new[,] {{0, 0, 0, 0}, {1, 1, 1, 1}, {0, 0, 0, 0}, {0, 0, 0, 0}}),
            _ => throw new ArgumentOutOfRangeException(nameof(shape), shape, null)
        };

        /// <summary>
        /// Tetromino's have specific colors.
        /// Here those colors are bound to the corresponding enum shape. 
        /// </summary>
        /// <param name="shape">TetrominoShape enum</param>
        /// <returns>color that corresponds with the given tetronimo shope</returns>
        /// <exception cref="ArgumentOutOfRangeException">when an invalid entry is passed</exception>
        public static Brush DetermineColor(TetrominoShape shape) => shape switch
        {
            O => Brushes.Yellow,
            T => Brushes.BlueViolet,
            J => Brushes.Aqua,
            L => Brushes.DarkOrange,
            S => Brushes.LimeGreen,
            Z => Brushes.Red,
            I => Brushes.Blue,
            _ => throw new ArgumentOutOfRangeException(nameof(shape), shape, null)
        };
    }
}