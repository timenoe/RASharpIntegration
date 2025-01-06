using System.Text.Json.Serialization;

namespace RAStandaloneIntegration.Network
{
    /// <summary>
    /// Generic API response from RA
    /// </summary>
    /// <typeparam name="T">Type of the response</typeparam>
    /// <param name="response">Deserialized response</param>
    /// <param name="failure">Failure message; "" if none</param>
    public class ApiResponse<T>(T? response, string failure = "")
    {
        /// <summary>
        /// Deserialized response
        /// </summary>
        public T? Response = response;

        /// <summary>
        /// Failure message; "" if none
        /// </summary>
        public string Failure = failure;

        /// <summary>
        /// Returns a successful API response instance
        /// </summary>
        /// <param name="response">Deserialized response</param>
        /// <returns></returns>
        public static ApiResponse<T> Succeded(T response)
        {
            return new ApiResponse<T>(response);
        }

        /// <summary>
        /// Returns a failed API response instance
        /// </summary>
        /// <param name="failure">Failure message</param>
        /// <returns></returns>
        public static ApiResponse<T> Failed(string failure)
        {
            return new ApiResponse<T>(default, failure);
        }
    }

    /// <summary>
    /// Base API response included in all API requests from RA<br/>
    /// Success is always included, Error will be included if applicable<br/>
    /// Use by directly deserializing the received JSON
    /// </summary>
    public class BaseResponse
    {
        /* ping success response example
            "Success": true
        */
        /* ping error response example
            "Success": false,
            "Status": 401,
            "Code": "invalid_credentials",
            "Error": "Invalid user/password combination. Please try again."
        */

        /// <summary>
        /// True if the request completed successfully
        /// </summary>
        [JsonPropertyName("Success")]
        public bool Success { get; set; }

        /// <summary>
        /// Error message if applicable
        /// </summary>
        [JsonPropertyName("Error")]
        public string? Error { get; set; }
    }

    /// <summary>
    /// API response from a login2 request to RA<br/>
    /// Use by directly deserializing the received JSON
    /// </summary>
    public class LoginResponse : BaseResponse
    {
        /* login2 response example
            "Success": true,
            "User": "OldSchoolRunescape",
            "Token": "4AotgGxjIH5iT1gz", // Store this token in ModConfig
            "Score": 1,
            "SoftcoreScore": 0,
            "Messages": 0,
            "Permissions": 1,
            "AccountType": "Registered"
        */

        /// <summary>
        /// Number of unread messages
        /// </summary>
        [JsonPropertyName("Messages")]
        public int Messages { get; set; }

        /// <summary>
        /// Site permissions identifier<br/>
        /// -2 = Spam<br/>
        /// -1 = Banned<br/>
        /// 0  = Unregistered<br/>
        /// 1  = Registered<br/>
        /// 2  = Junior Developer<br/>
        /// 3  = Developer<br/>
        /// 4  = Moderator<br/>
        /// 5  = Admin<br/>
        /// 6  = Root
        /// </summary>
        [JsonPropertyName("Permissions")]
        public int Perms { get; set; }

        /// <summary>
        /// Hardcore points
        /// </summary>
        [JsonPropertyName("Score")]
        public int Score { get; set; }

        /// <summary>
        /// Softcore points
        /// </summary>
        [JsonPropertyName("SoftcoreScore")]
        public int SoftScore { get; set; }

        /// <summary>
        /// Account type identifier<br/>
        /// Can be Spam, Banned, Unregistered, Registered, Junior Developer, Developer, Moderator, or Invalid permission
        /// </summary>
        [JsonPropertyName("AccountType")]
        public string? AccountType { get; set; }

        /// <summary>
        /// Connect token
        /// </summary>
        [JsonPropertyName("Token")]
        public string? Token { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        [JsonPropertyName("User")]
        public string? User { get; set; }
    }

    /// <summary>
    /// API response from a startsession request to RA<br/>
    /// Use by directly deserializing the received JSON
    /// </summary>
    public class StartSessionResponse : BaseResponse
    {
        /* startsession response example
            "Success": true,
            "HardcoreUnlocks": [
            {
                "ID": 141,
                "When": 1591132445
            }
            ],
            "ServerNow": 1704076711
        */

        /// <summary>
        /// Epoch time of the server
        /// </summary>
        [JsonPropertyName("ServerNow")]
        public int ServerTime { get; set; }

        /// <summary>
        /// Hardcore unlock timestamps for the game
        /// </summary>
        [JsonPropertyName("HardcoreUnlocks")]
        public HardcoreUnlock[]? HardcoreUnlocks { get; set; }

        /// <summary>
        /// Used to identify a hardcore unlock timestamp
        /// </summary>
        public class HardcoreUnlock
        {
            [JsonPropertyName("ID")]
            public int Id { get; set; }

            [JsonPropertyName("When")]
            public int When { get; set; }
        }

        /// <summary>
        /// Get an list of unlocked achievement IDs, disregarding the timestamps
        /// </summary>
        /// <returns>List of unlocked achievement IDs</returns>
        public List<int> GetUnlockedAchIds()
        {
            List<int> unlockedAchs = [];
            if (HardcoreUnlocks != null)
            {
                foreach (HardcoreUnlock unlock in HardcoreUnlocks)
                    unlockedAchs.Add(unlock.Id);
            }
            return unlockedAchs;
        }
    }

    /// <summary>
    /// API response from an awardachievement request to RA<br/>
    /// Use by directly deserializing the received JSON
    /// </summary>
    public class AwardAchievementResponse : BaseResponse
    {
        /* awardachievement response example
            "Success": true,
            "AchievementsRemaining": 5,
            "Score": 22866,
            "SoftcoreScore": 5,
            "AchievementID": 9 // New unlock
        */

        /// <summary>
        /// Achievements remaining for the game; mastered if 0
        /// </summary>
        [JsonPropertyName("AchievementsRemaining")]
        public int AchsRemaining { get; set; }

        /// <summary>
        /// Achievement ID that was successfully unlocked
        /// </summary>
        [JsonPropertyName("AchievementID")]
        public int AchId { get; set; }

        /// <summary>
        /// Hardcore points
        /// </summary>
        [JsonPropertyName("Score")]
        public int Score { get; set; }

        /// <summary>
        /// Softcore points
        /// </summary>
        [JsonPropertyName("SoftcoreScore")]
        public int SoftScore { get; set; }
    }

    /// <summary>
    /// API response from an awardachievements request to RA<br/>
    /// Use by directly deserializing the received JSON
    /// </summary>
    public class AwardAchievementsResponse : BaseResponse
    {
        /* awardachievements response example
            "Success": true,
            "Score": 22890,
            "SoftcoreScore": 5,
            "ExistingIDs": [141, 147],
            "SuccessfulIDs": [142, 145, 146]
        */

        /// <summary>
        /// Hardcore score
        /// </summary>
        [JsonPropertyName("Score")]
        public int Score { get; set; }

        /// <summary>
        /// Softcore score
        /// </summary>
        [JsonPropertyName("SoftcoreScore")]
        public int SoftScore { get; set; }

        /// <summary>
        /// Achievement IDs that were already unlocked
        /// </summary>
        [JsonPropertyName("ExistingIDs")]
        public List<int>? ExistingIds { get; set; }

        /// <summary>
        /// Achievement IDs that were successfully unlocked
        /// </summary>
        [JsonPropertyName("SuccessfulIDs")]
        public List<int>? SuccessfulIds { get; set; }
    }

    /// <summary>
    /// API response from an uploadachievement request to RA<br/>
    /// Use by directly deserializing the received JSON
    /// </summary>
    public class UploadAchievementResponse : BaseResponse
    {
        /* uploadachievement response example
            "Success": true,
            "AchievementID": 483244,
        */

        /// <summary>
        /// Achievement ID that was successfully uploaded/updated
        /// </summary>
        [JsonPropertyName("AchievementID")]
        public int AchId { get; set; }
    }
}
