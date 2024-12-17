namespace Minesweeper.Core.DataModels
{
    /// <summary>
    /// The possible states of the <see cref="Tile"/>
    /// </summary>
    public enum TileState
    {
        /// <summary>
        /// States that the tile has not been opened.
        /// </summary>
        Covered,

        /// <summary>
        /// States that it is unclear whether the tile contains a bomb or not.
        /// </summary>
        QuestionMarked,

        /// <summary>
        /// States that the tile has been flagged for having a bomb in it.
        /// </summary>
        Flagged,

        /// <summary>
        /// States that the <see cref="Tile"/> has been opened and might be showing the number of surrounding bombs.
        /// </summary>
        Unconvered
    }
}