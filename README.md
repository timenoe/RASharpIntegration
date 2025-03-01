# RASharpIntegration

[![Issues](https://img.shields.io/github/issues/timenoe/RASharpIntegration?style=flat-square)](https://github.com/timenoe/RASharpIntegration/issues)
[![License](https://img.shields.io/github/license/timenoe/RASharpIntegration?style=flat-square)](https://github.com/timenoe/RASharpIntegration/blob/master/LICENSE.md)

C# Library to integrate a standalone client with RetroAchievements.

Target Frameworks: `.NET 5.0, .NET 6.0, .NET 7.0, .NET 8.0, .NET 9.0`

Unit tests are located at [RASharpIntegration Tests](https://github.com/timenoe/RASharpIntegrationTests).

## How to Use

1. Add this repository as a submodule to your C# project.

2. Add `using RASharpIntegration.Network` to a C# file.

3. Create a `RequestHeader` and populate it with the required information (`host`, `game`, `hardcore`).

4. Create an `HttpClient` to make requests with. Set a User Agent that RA will accept. Example:

```csharp
HttpClient _client;
_client.DefaultRequestHeaders.Add("User-Agent", $"{ClientName}/{Version}");
```

4. Call the static methods within `NetworkInterface` to make requests to the RA host. Example:

```csharp
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
