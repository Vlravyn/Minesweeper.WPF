using MvvmEssentials.Core;
using System.Text.Json.Serialization;

namespace Minesweeper.Core.DataModels
{
    /// <summary>
    /// Stores data according to the <see cref="GameDifficulty"/>
    /// </summary>
    public class StatsForDifficultyHost : ObservableObject
    {
        private ulong _gamesPlayed;
        private ulong _gamesWon;
        private ulong? _bestTime;
        private ulong _longestWinningStreak;
        private ulong _longestLosingStreak;
        private ulong _currentWinningStreak;
        private ushort _winningPercentage;

        /// <summary>
        /// Constructor or json
        /// </summary>
        [JsonConstructor]
        public StatsForDifficultyHost(ulong gamesPlayed, ulong gamesWon, ulong? bestTime, ulong longestWinningStreak, ulong longestLosingStreak, ulong currentWinningStreak, ushort winningPercentage)
        {
            GamesPlayed = gamesPlayed;
            GamesWon = gamesWon;
            BestTime = bestTime;
            LongestWinningStreak = longestWinningStreak;
            LongestLosingStreak = longestLosingStreak;
            CurrentWinningStreak = currentWinningStreak;
            WinningPercentage = winningPercentage;
        }

        /// <summary>
        /// Creates an instance of <see cref="StatsForDifficultyHost"/>
        /// </summary>
        internal StatsForDifficultyHost(GameDifficultyHost difficulty)
        {
            Difficulty = difficulty;
        }

        public GameDifficultyHost Difficulty { get; internal set; }

        /// <summary>
        /// The number of games played.
        /// </summary>
        public ulong GamesPlayed
        {
            get => _gamesPlayed;
            internal set
            {
                SetProperty(ref _gamesPlayed, value);
                if (GamesPlayed > 0)
                    WinningPercentage = (ushort)((GamesWon / GamesPlayed) * 100);
            }
        }

        /// <summary>
        /// The number of games won.
        /// </summary>
        public ulong GamesWon
        {
            get => _gamesWon;
            internal set => SetProperty(ref _gamesWon, value);
        }

        /// <summary>
        /// Winning percentage for this difficulty.
        /// </summary>
        public ushort WinningPercentage
        {
            get => _winningPercentage;
            internal set => SetProperty(ref _winningPercentage, value);
        }

        /// <summary>
        /// The best time that has be achieved yet.
        /// </summary>
        public ulong? BestTime
        {
            get => _bestTime;
            internal set => SetProperty(ref _bestTime, value);
        }

        /// <summary>
        /// The longest winning streak for this difficulty.
        /// </summary>
        public ulong LongestWinningStreak
        {
            get => _longestWinningStreak;
            internal set => SetProperty(ref _longestWinningStreak, value);
        }

        /// <summary>
        /// The longest losing streak for this difficulty.
        /// </summary>
        public ulong LongestLosingStreak
        {
            get => _longestLosingStreak;
            internal set => SetProperty(ref _longestLosingStreak, value);
        }

        /// <summary>
        /// Current winning streak on this difficulty.
        /// </summary>
        public ulong CurrentWinningStreak
        {
            get => _currentWinningStreak;
            internal set => SetProperty(ref _currentWinningStreak, value);
        }
    }
}