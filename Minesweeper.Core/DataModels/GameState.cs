namespace Minesweeper.Core.DataModels
{
    /// <summary>
    /// Represents all the possible states of the game.
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// States that the game has just started and no mines have been opened.
        /// </summary>
        NewGame,

        /// <summary>
        /// States that the game is being played.
        /// </summary>
        InProgress,

        /// <summary>
        /// States that the game has ended.
        /// </summary>
        GameEnd
    }
}