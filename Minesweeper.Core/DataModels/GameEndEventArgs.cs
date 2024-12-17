namespace Minesweeper.Core.DataModels
{
    /// <summary>
    /// The events arguments for when the game ends.
    /// </summary>
    public class GameEndEventArgs
    {
        /// <summary>
        /// Whether the game was completed or not.
        /// </summary>
        public bool GameWon { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="GameEndEventArgs"/>
        /// </summary>
        /// <param name="gameWon">Whether the game was won or not.</param>
        public GameEndEventArgs(bool gameWon)
        {
            GameWon = gameWon;
        }
    }
}