using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using RASharpIntegration.Data;

namespace RASharpIntegration.Network
{
    /// <summary>
    /// Interface to send API requests and get the reponse from RA
    /// </summary>
    public class NetworkInterface
    {
        /// <summary>
        /// Try to login a user
        /// </summary>
        /// <param name="client">HTTP client to send the request from</param>
        /// <param name="header">Request header containing the host and user</param>
        /// <param name="pass">User password</param>
        /// <returns>RA API response from a login request</returns>
        public static async Task<ApiResponse<LoginResponse>> TryLogin(HttpClient client, RequestHeader header, string pass)
        {
            Uri request = NetworkRequest.BuildLoginRequest(header, pass);
            return await GetApiResponse<LoginResponse>(client, request);
        }

        /// <summary>
        /// Try to start a session for the game
        /// </summary>
        /// <param name="client">HTTP client to send the request from</param>
        /// <param name="header">Request header containing the host, user, and token</param>
        /// <returns>RA API response from a start session request</returns>
        public static async Task<ApiResponse<StartSessionResponse>> TryStartSession(HttpClient client, RequestHeader header, int game)
        {
            Uri request = NetworkRequest.BuildStartSessionRequest(header, game);
            return await GetApiResponse<StartSessionResponse>(client, request);
        }

        /// <summary>
        /// Try to send an activity ping for the game
        /// </summary>
        /// <param name="client">HTTP client to send the request from</param>
        /// <param name="header">Request header containing the host, user, and token</param>
        /// <param name="rp">Rich presence</param>
        /// <returns></returns>
        public static async Task<ApiResponse<BaseResponse>> TryPing(HttpClient client, RequestHeader header, string rp)
        {
            Uri request = NetworkRequest.BuildPingRequest(header, rp, out MultipartFormDataContent multipart);
            return await GetApiResponse<BaseResponse>(client, request, multipart);
        }

        /// <summary>
        /// Try to award an achievement for the user
        /// </summary>
        /// <param name="client">HTTP client to send the request from</param>
        /// <param name="header">Request header containing the host, user, and token</param>
        /// <param name="ach">Achievement ID</param>
        /// <returns></returns>
        public static async Task<ApiResponse<AwardAchievementResponse>> TryAwardAchievement(HttpClient client, RequestHeader header, int ach)
        {
            Uri request = NetworkRequest.BuildAwardAchievementRequest(header, ach);
            return await GetApiResponse<AwardAchievementResponse>(client, request);
        }

        /// <summary>
        /// Try to award a batch of achievements for the user<br/>
        /// Can only be used in delegated standalones (Terraria is not, for example)
        /// </summary>
        /// <param name="client">HTTP client to send the request from</param>
        /// <param name="header">Request header containing the host, user, and token</param>
        /// <param name="achs">Achievement IDs</param>
        /// <returns></returns>
        public static async Task<ApiResponse<AwardAchievementsResponse>> TryAwardAchievements(HttpClient client, RequestHeader header, List<int> achs)
        {
            Uri request = NetworkRequest.BuildAwardAchievementsRequest(header, achs, out MultipartFormDataContent multipart);
            return await GetApiResponse<AwardAchievementsResponse>(client, request, multipart);
        }

        /// <summary>
        /// Try to upload a game achievement<br/>
        /// Also used to update an existing achievement if an achievement ID is provided
        /// </summary>
        /// <param name="client">HTTP client to send the request from</param>
        /// <param name="header">Request header containing the host, user, and token</param>
        /// <param name="game">Game ID</param>
        /// <param name="ach">Achievement information</param>
        /// <returns></returns>
        public static async Task<ApiResponse<UploadAchievementResponse>> TryUploadAchievement(HttpClient client, RequestHeader header, RaAchievement ach)
        {
            Uri request = NetworkRequest.BuildUploadAchievementRequest(header, ach);
            return await GetApiResponse<UploadAchievementResponse>(client, request);
        }

        /// <summary>
        /// Send an API request and get the response
        /// </summary>
        /// <typeparam name="T">Type of response to receive</typeparam>
        /// <param name="client">HTTP client to send the request from</param>
        /// <param name="request">Complete URI for the request</param>
        /// <param name="mp">Multipart Form Data to send with the request</param>
        /// <returns></returns>
        private static async Task<ApiResponse<T>> GetApiResponse<T>(HttpClient client, Uri request, MultipartFormDataContent mp = default)
        {
            try
            {
                // Console.WriteLine($"Raw API request: {request}");
                HttpResponseMessage response = await client.PostAsync(request, mp);
                string content = await response.Content.ReadAsStringAsync();
                // Console.WriteLine($"Raw API response: {content}");

                T data = JsonSerializer.Deserialize<T>(content);
                if (data == null)
                    return ApiResponse<T>.Failed($"Deserialized JSON response is null");
                return ApiResponse<T>.Succeded(data);
            }

            // HttpRequestException can occur if the client's internet is down, there are DNS issues, etc.
            // JsonException can occur if the JSON response from the RA server is invalid
            // There are many other exceptions that could occur in a bad environment
            // In this case, catch all possible exceptions and log as a failure
            // Let the client decide how they want to handle failures
            catch (Exception e)
            {
                if (e is JsonException)
                    return ApiResponse<T>.Failed("Invalid server response");

                return ApiResponse<T>.Failed(e.Message);
            }
        }
    }
}
