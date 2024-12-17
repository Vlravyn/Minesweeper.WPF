namespace Minesweeper.Core.DataModels
{
    /// <summary>
    /// Hosts data about <see cref="GameDifficulty"/>
    /// </summary>
    public class GameDifficultyHost
    {
        /// <summary>
        /// Preset for <see cref="GameDifficulty.Easy"/>
        /// </summary>
        public static GameDifficultyHost Easy { get; } = new GameDifficultyHost()
        {
            Rows = 9,
            Columns = 9,
            Mines = 10,
            DifficultyType = GameDifficulty.Easy
        };

        /// <summary>
        /// Preset for <see cref="GameDifficulty.Medium"/>
        /// </summary>
        public static GameDifficultyHost Medium { get; } = new GameDifficultyHost()
        {
            Rows = 16,
            Columns = 16,
            Mines = 40,
            DifficultyType = GameDifficulty.Medium
        };

        /// <summary>
        /// Preset for <see cref="GameDifficulty.Hard"/>
        /// </summary>
        public static GameDifficultyHost Hard { get; } = new GameDifficultyHost()
        {
            Rows = 16,
            Columns = 30,
            Mines = 99,
            DifficultyType = GameDifficulty.Hard
        };

        /// <summary>
        /// The DifficultyType for this difficulty.
        /// </summary>
        /// <remarks>
        /// By default this property is set to custom because all the difficulties creates outside of this class will be custom.
        /// And class already has public properties ofr the other game difficulties
        /// </remarks>
        public GameDifficulty DifficultyType { get; private set; } = GameDifficulty.Custom;

        /// <summary>
        /// The rows that this difficulty should have.
        /// </summary>
        public ushort Rows { get; init; }

        /// <summary>
        /// The columns that this difficulty should have.
        /// </summary>
        public ushort Columns { get; init; }

        /// <summary>
        /// The minesthat this difficulty should have.
        /// </summary>
        public ushort Mines { get; init; }

        /// <summary>
        /// Creates an instance of <see cref="GameDifficultyHost"/>
        /// </summary>
        public GameDifficultyHost()
        {
        }
    }
}