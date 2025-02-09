# RASharpIntegration

C# Library to integrate a standalone client with RetroAchievements.

[Unit Tests](https://github.com/timenoe/RASharpIntegrationTests)

## How to Use

1. Add this repository as a submodule to your C# project.

2. Add `using RASharpIntegration.Network` to a C# file.

3. Create a `RequestHeader` and populate it with the required information (`host`, `game`, `hardcore`).

4. Create an `HttpClient` to make requests with. Set a User Agent that RA will accept. Example:

```
HttpClient _client;
_client.DefaultRequestHeaders.Add("User-Agent", $"{ClientName}/{Version}");
```

4. Call the static methods within `NetworkInterface` to make requests to the RA host. Example:

```
private async Task Login(string user, string pass)
{
    _header.user = user;
    MessageUtil.Log($"Logging in {user} to {_header.host}...");
    ApiResponse<LoginResponse> api = await NetworkInterface.TryLogin(_client, _header, pass);

    if (!string.IsNullOrEmpty(api.Failure))
    {
        MessageUtil.Log($"Unable to login ({api.Failure})");
        return;
    }

    if (!api.Response.Success)
    {
        MessageUtil.Log($"Unable to login ({api.Response.Error})");
        return;
    }
        
    _header.token = api.Response.Token;
    CacheCredentials();

    MessageUtil.Log($"{user} has successfully logged in!");
    StartSessionCommand.Invoke(this, null);
}
```