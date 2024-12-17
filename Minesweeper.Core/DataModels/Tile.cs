using MvvmEssentials.Core;

namespace Minesweeper.Core.DataModels
{
    /// <summary>
    /// Represents a single tile in the game
    /// </summary>
    public class Tile : ObservableObject
    {
        private TileState _tileState;
        private int _row;
        private int _column;
        private int _adjacentMinesCount;

        /// <summary>
        /// The state of the tile
        /// </summary>
        public TileState TileState
        {
            get => _tileState;
            internal set => SetProperty(ref _tileState, value);
        }

        /// <summary>
        /// The row that this tile is on.
        /// </summary>
        public int Row
        {
            get => _row;
            private init => SetProperty(ref _row, value);
        }

        /// <summary>
        /// The column that this tile is on.
        /// </summary>
        public int Column
        {
            get => _column;
            private init => SetProperty(ref _column, value);
        }

        /// <summary>
        /// Represents whether this tile contains a bomb or not.
        /// </summary>
        public bool ContainsBomb { get; set; }

        /// <summary>
        /// Contains the count of <see cref="Tile"/> with <see cref="ContainsBomb"/> set to <see langword="true"/> surround this tile.
        /// </summary>
        public int AdjacentMinesCount
        {
            get => _adjacentMinesCount;
            set => SetProperty(ref _adjacentMinesCount, value);
        }

        /// <summary>
        /// Creates a new instance of <see cref="Tile"/>
        /// </summary>
        /// <param name="tileState">the state of the tile</param>
        /// <param name="row">the row that this tile is on</param>
        /// <param name="column">the column that this tile is on</param>
        internal Tile(TileState tileState, ushort row, ushort column)
        {
            TileState = tileState;
            Row = row;
            Column = column;
            ContainsBomb = false;
        }
    }
}