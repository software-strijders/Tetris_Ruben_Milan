namespace TetrisClient
{
    public class Score
    {
        public int Level;
        public int Points;
        public int Rows;

        public Score()
        {
            this.Level = 0;
            this.Points = 0;
            this.Rows = 0;
        }

        public void HandleScore(int rows)
        {
            if (rows == 0) return;

            var multiplier = rows switch
            {
                1 => 40,
                2 => 100,
                3 => 300,
                _ => 1200
            };

            this.Points += this.Level * multiplier + multiplier;
            this.Rows += rows;
        }

        public void HandleLevel()
        {
            if (this.Rows == 0) return;
            
            if (this.Rows % 10 == 0)
            {
                this.Level++;
            }
        }
    }
}