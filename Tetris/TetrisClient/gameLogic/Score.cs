namespace TetrisClient
{
    public class Score
    {
        public int Level;

        public int Points;

        //number of deleted rows
        public int Rows;

        /// <summary>
        /// used for level calculation
        /// if it reaches 10 or higher the level is upped
        /// </summary>
        private int _rowsForLeveling;

        public Score()
        {
            Level = 0;
            Points = 0;
            Rows = 0;
        }

        /// <summary>
        /// Calculates the score that needs to be added when <paramref name="rows"/> are deleted
        /// </summary>
        /// <param name="rows"></param>
        public void HandleScore(int rows)
        {
            if (rows == 0) return;

            var multiplier = rows switch
            {
                2 => 100,
                3 => 300,
                4 => 1200,
                _ => 40,
            };

            Points += Level * multiplier + multiplier;
            Rows += rows;
            _rowsForLeveling += rows;
        }

        /// <summary>
        /// Calculates if a level needs to be added (level will be upped by one every 10 deleted rows)
        /// </summary>
        /// <returns>true if the level is upped else false</returns>
        public bool HandleLevel()
        {
            if (_rowsForLeveling < 10) return false;
            _rowsForLeveling -= 10;
            Level++;
            return true;
        }
    }
}