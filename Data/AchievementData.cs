﻿namespace RASharpIntegration.Data
{
    // Output out Edit > Paste Special > Paste JSON as classes (renamed for accuracy)

    /// <summary>
    /// Used to idenity a game on RA<br/><br/>
    /// 
    /// Can be used to directly deserialize a custom JSON file containing achievement information<br/>
    /// Check the Terraria RetroAchievements Mod for an implementation example
    /// </summary>
    public class RaGame
    {
        /// <summary>
        /// ID of the game
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the game
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// IDs of the sets associated with this game<br/>
        /// Includes the base set and any subsets
        /// </summary>
        public int[] Sets { get; set; }
    }

    /// <summary>
    /// Used to identify an achievement on RA<br/><br/>
    /// 
    /// Can be used to directly deserialize a custom JSON file containing achievement information<br/>
    /// Check the Terraria RetroAchievements Mod for an implementation example
    /// </summary>
    public class RaAchievement
    {
        /// <summary>
        /// Title of the achievement
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Description of the achievement
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Points of the achievement<br/>
        /// Must be 0, 1, 2, 3, 4, 5, 10, 25, 50, or 100
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// Type of the achievement<br/>
        /// Must be missable, progression, or win_condition; "" if none
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Category of the achievement<br/>
        /// Must be 5 for Unoffical or 3 for Core
        /// </summary>
        public int Category { get; set; }

        /// <summary>
        /// ID of the achievement; 0 if none
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Badge ID of the achievement; "00000" is the default badge
        /// </summary>
        public string Badge { get; set; }

        /// <summary>
        /// Game ID of the set that the achievement is in
        /// </summary>
        public int Set { get; set; }
    }
}
