using Minesweeper.Core.DataModels;
using MvvmEssentials.Core;
using System.Text.Json;

namespace Minesweeper.Core
{
    /// <summary>
    /// Statistics for this game.
    /// </summary>
    public class Statistics : ObservableObject
    {
        private static bool isBeingDeserialized = false;
        private static readonly string FolderLocation = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Minesweeper";
        private static readonly string StatisticsFileLocation = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Minesweeper\\Statistics.json";

        private readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            WriteIndented = true,
        };

        /// <summary>
        /// Statistics for <see cref="GameDifficulty.Easy"/>
        /// </summary>
        public StatsForDifficultyHost EasyDifficulty { get; set; }

        /// <summary>
        /// Statistics for <see cref="GameDifficulty.Medium"/>
        /// </summary>
        public StatsForDifficultyHost MediumDifficulty { get; set; }

        /// <summary>
        /// Statistics for <see cref="GameDifficulty.Hard"/>
        /// </summary>
        public StatsForDifficultyHost HardDifficulty { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="Statistics"/>
        /// </summary>
        public Statistics()
        {
            Directory.CreateDirectory(FolderLocation);

            //Attempt to get old saved statistics
            if (File.Exists(StatisticsFileLocation) && isBeingDeserialized is false)
            {
                isBeingDeserialized = true;
                var json = File.ReadAllText(StatisticsFileLocation);
                var Instance = JsonSerializer.Deserialize<Statistics>(json, jsonSerializerOptions);

                //if the old statistics were found, set this instance to that old instance.
                //Otherwise, just create a new instance with default values.
                if (Instance != null)
                {
                    EasyDifficulty = Instance.EasyDifficulty;
                    MediumDifficulty = Instance.MediumDifficulty;
                    HardDifficulty = Instance.HardDifficulty;
                }
            }

            EasyDifficulty ??= new(GameDifficultyHost.Easy);
            MediumDifficulty ??= new(GameDifficultyHost.Medium);
            HardDifficulty ??= new(GameDifficultyHost.Hard);
        }

        /// <summary>
        /// Saves the current statistics to the local application folder.
        /// </summary>
        public void SaveStatistics()
        {
            var json = JsonSerializer.Serialize(this, jsonSerializerOptions);
            File.WriteAllText(StatisticsFileLocation, json);
        }

        public void ResetStatistics()
        {
            EasyDifficulty = new(GameDifficultyHost.Easy);
            MediumDifficulty = new(GameDifficultyHost.Medium);
            HardDifficulty = new(GameDifficultyHost.Hard);
            File.Delete(StatisticsFileLocation);
        }
    }
}