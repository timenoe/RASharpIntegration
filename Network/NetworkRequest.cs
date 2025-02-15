using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using RASharpIntegration.Data;

namespace RASharpIntegration.Network
{
    /// <summary>
    /// Information that is commonly used in requests to RA
    /// </summary>
    /// <param name="host">Host name (retroachievements.org, etc.)</param>
    /// <param name="game">Game ID</param>
    /// <param name="hardcore">Hardcore status</param>
    /// <param name="user">User name</param>
    /// <param name="token">Connect token</param>
    public class RequestHeader(string host, int game, bool hardcore, string user = "", string token = "")
    {
        public string host = host;
        public int game = game;
        public bool hardcore = hardcore;
        public string user = user;
        public string token = token;
    }

    /// <summary>
    /// Used to build API requests for RA
    /// </summary>
    public class NetworkRequest
    {
        /// <summary>
        /// Build a URI for a login2 request
        /// </summary>
        /// <param name="header">Request header containing common required parameters</param>
        /// <param name="pass">User password</param>
        /// <returns>Complete URI for a login2 request</returns>
        public static Uri BuildLoginRequest(RequestHeader header, string pass)
        {
            UriBuilder builder = new($"https://{header.host}/dorequest.php");
            NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);

            parameters["u"] = header.user;
            parameters["r"] = "login2";
            parameters["p"] = pass;
            builder.Query = parameters.ToString();
            return builder.Uri;
        }

        /// <summary>
        /// Build a URI for a startsession request
        /// </summary>
        /// <param name="header">Request header containing common required parameters</param>
        /// <param name="game">Game ID</param>
        /// <returns>Complete URI for a startsession request</returns>
        public static Uri BuildStartSessionRequest(RequestHeader header, int game)
        {
            UriBuilder builder = new($"https://{header.host}/dorequest.php");
            NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);

            parameters["u"] = header.user;
            parameters["t"] = header.token;
            parameters["r"] = "startsession";
            parameters["g"] = game.ToString();
            builder.Query = parameters.ToString();
            return builder.Uri;
        }

        /// <summary>
        /// Build a URI for a ping request
        /// </summary>
        /// <param name="header">Request header containing common required parameters</param>
        /// <param name="rp">Rich presence</param>
        /// <param name="mp">Multipart Form Data for the rich presence</param>
        /// <returns>Complete URI for a ping request</returns>
        public static Uri BuildPingRequest(RequestHeader header, string rp, out MultipartFormDataContent mp)
        {
            UriBuilder builder = new($"https://{header.host}/dorequest.php");
            mp = [];
            NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);

            parameters["u"] = header.user;
            parameters["t"] = header.token;
            parameters["r"] = "ping";
            parameters["g"] = header.game.ToString();
            mp.Add(new StringContent(rp), "m");
            builder.Query = parameters.ToString();
            return builder.Uri;
        }

        /// <summary>
        /// Build a URI for a awardachievement request
        /// </summary>
        /// <param name="header">Request header containing common required parameters</param>
        /// <param name="ach">Achievement ID</param>
        /// <returns>Complete URI for an awardachievement request</returns>
        public static Uri BuildAwardAchievementRequest(RequestHeader header, int ach)
        {
            UriBuilder builder = new($"https://{header.host}/dorequest.php");
            NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);

            parameters["u"] = header.user;
            parameters["t"] = header.token;
            parameters["r"] = "awardachievement";
            parameters["h"] = header.hardcore ? "1" : "0";
            parameters["a"] = ach.ToString();
            parameters["v"] = GenerateVerifyMD5($"{ach}{header.user}{header.hardcore}{ach}");
            builder.Query = parameters.ToString();
            return builder.Uri;
        }

        /// <summary>
        /// Build a URI for a awardachievements request
        /// </summary>
        /// <param name="header">Request header containing common required parameters</param>
        /// <param name="achs">Achievement ID</param>
        /// <param name="mp">Multipart Form Data for the achievement unlocks</param>
        /// <returns>Complete URI for an awardachievements request</returns>
        public static Uri BuildAwardAchievementsRequest(RequestHeader header, List<int> achs, out MultipartFormDataContent mp)
        {
            UriBuilder builder = new($"https://{header.host}/dorequest.php");
            mp = [];
            NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);

            parameters["u"] = header.user;
            parameters["t"] = header.token;
            parameters["r"] = "awardachievements";
            mp.Add(new StringContent(header.hardcore ? "1" : "0"), "h");
            mp.Add(new StringContent(string.Join(",", achs)), "a");
            mp.Add(new StringContent(GenerateVerifyMD5($"{achs}{header.user}{header.hardcore}")), "v");
            builder.Query = parameters.ToString();
            return builder.Uri;
        }

        /// <summary>
        /// Build a URI for a uploadachievement request<br/>
        /// </summary>
        /// <param name="header">Request header containing common required parameters</param>
        /// <param name="ach">Achievement information</param>
        /// <returns>Complete URI for an uploadachievement request</returns>
        public static Uri BuildUploadAchievementRequest(RequestHeader header, RaAchievement ach)
        {
            UriBuilder builder = new($"https://{header.host}/dorequest.php");
            NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);

            parameters["u"] = header.user;
            parameters["t"] = header.token;
            parameters["r"] = "uploadachievement";
            parameters["g"] = header.game.ToString();
            parameters["n"] = ach.Title;
            parameters["d"] = ach.Description;
            parameters["z"] = ach.Points.ToString();
            parameters["x"] = ach.Type;
            parameters["m"] = "0=1"; // Designed for Standalones with no trigger logic; change as needed
            parameters["f"] = ach.Category.ToString();
            parameters["a"] = ach.Id.ToString();
            parameters["b"] = ach.Badge;
            builder.Query = parameters.ToString();
            return builder.Uri;
        }

        /// <summary>
        /// Generate a verification MD5 hash used in achievement unlock requests
        /// </summary>
        /// <param name="str">Unique string to hash</param>
        /// <returns>Verification MD5 hash</returns>
        private static string GenerateVerifyMD5(string str)
        {
            return string.Join("", MD5.HashData(Encoding.ASCII.GetBytes(str)).Select(s => s.ToString("x2")));
        }
    }
}
