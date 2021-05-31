using System;
using System.Windows.Media;
using static TetrisClient.TetronimoShape;

namespace TetrisClient
{
    public enum TetronimoShape
    {
        O,
        T,
        J,
        L,
        S,
        Z,
        I
    }

    public class Tetronimo
    {
        public TetronimoShape shape { get; private set; }
        public int[,] IntArray { get; set; }
        private int offsetX { get; set; }
        private int offsetY { get; set; }

        public Tetronimo() => new Tetronimo(0, 0);

        public Tetronimo(int offsetX, int offsetY)
        {
            var generatedShape = GenerateShape();
            shape = generatedShape;
            IntArray = CreateShape(generatedShape);
            this.offsetX = offsetX;
            this.offsetY = offsetY;
        }

        private static TetronimoShape GenerateShape()
        {
            var values = Enum.GetValues(typeof(TetronimoShape));
            return (TetronimoShape) values.GetValue(new Random().Next(values.Length));
        }

        private static int[,] CreateShape(TetronimoShape shape) => shape switch
        {
            O => new[,] {{1, 1}, {1, 1}},
            T => new[,] {{1, 1, 1}, {0, 1, 0}, {0, 0, 0}},
            J => new[,] {{0, 1, 0}, {0, 1, 0}, {1, 1, 0}},
            L => new[,] {{0, 1, 0}, {0, 1, 0}, {0, 1, 1}},
            S => new[,] {{0, 1, 1}, {1, 1, 0}, {0, 0, 0}},
            Z => new[,] {{1, 1, 0}, {0, 1, 1}, {0, 0, 0}},
            I => new[,] {{0, 0, 0, 0}, {1, 1, 1, 1}, {0, 0, 0, 0}},
            _ => throw new ArgumentOutOfRangeException(nameof(shape), shape, null)
        };


        public static Brush DetermineColor(TetronimoShape shape) => shape switch
        {
            O => Brushes.Yellow,
            T => Brushes.Purple,
            J => Brushes.Blue,
            L => Brushes.Orange,
            S => Brushes.Green,
            Z => Brushes.Red,
            I => Brushes.Aqua,
            _ => throw new ArgumentOutOfRangeException(nameof(shape), shape, null)
        };
    }
}