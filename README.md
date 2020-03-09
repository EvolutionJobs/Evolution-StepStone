# Evolution-StepStone

![.NET Core](https://github.com/EvolutionJobs/Evolution-StepStone/workflows/.NET%20Core/badge.svg)

Evolution's DI library for connecting to V6.17.1 StepStone's Federated Candidate Search API.

This is a preview API and is not ready for production release.

## How to set up

Add a config section holding the connection details for the StepStone brand to `appsettings.json`:

```json
"StepStone": [
    {
        "Name":              "Optional, fallback to Url",
        "Url":               "https://recruiter.{StepStone brand site}",
        "ClientID":          "████████-████-████-████-████████████",
        "ClientSecret":      "████████-████-████-████-████████████",
        "RecruiterUsername": "████████",
        "RecruiterPassword": "████████"
    },
    ...
]
```

This can then be added using the standard Dependency Injection pattern in `Startup.cs`:

```c#
public void ConfigureServices(IServiceCollection services)
{
     ...
     services.AddStepStoneService("MyAppName", this.Configuration.GetSection("StepStone"));
```

This can then be referenced as a DI service: `[FromServices] IStepStoneService stepStone`

## How to use

Assuming you have the DI instance `IStepStoneService stepStone`...

### Authenticate
First you need to get the authentication token:

```c#
// Authenticate a new token, the username and key are your internal ones
var token = await stepStone.Authenticate("your username", "your session key");
```

This token can be re-used for up to `token.Expires` in seconds.

This token will hold the keys for all the services configured and is unique to the user, it should not be shared between users.

The username and session key are passed in the header for every request - they can be anything, but should uniquely identify the application user.

#### Authentication Errors

Authentication can time out and will throw a `StepStoneAuthenticationException` if a request is made with an outdated token.

Wrap calls to the API in a `try-catch` to handle this:

```c#
Func<StepStoneToken> action = /* Action calling StepStone API */;

try { return await action(token) }
catch (StepStoneAuthenticationException authEx)
{
    logger.LogError(authEx, "Authentication exception, getting a new token.");

    // Clear the old token and get a new one
    var newToken = await this.StepStoneToken(user);

    // Save this new token
 
    // Try again with a new token, but don't wrap in try-catch again
    return await action(newToken);
}
```

Don't use this pattern for the `Authenticate` method, as in that context `StepStoneAuthenticationException` means your credentials are incorrect.

### Quotas
To check a quota pass the token and the name (if configured) or the `https://recruiter.{StepStone brand site}`:

```c#
var quota = await stepStone.Quota(token, "brand name");
```

### Run a search
Build a [`SearchRequest`](Models/SearchRequest.cs) with the details of the search, then call:

```c#
SearchRequest request = /* Build search request object */;

var searchResults = await stepStone.Search(token, "brand name", request);
// OR, the last 2 flags are optional
var searchResults = await stepStone.Search(token, "brand name", request, includeFacets, includeCandidatesActivity);

int resultCount = searchResults.TotalResultsCount;

// If includeFacets: true passed to Search
var facets = searchResults.Facets;

foreach(var candidate in searchResults.Candidates) {
    // Unique ID to get the candidate's contact details or CV
    var id = candidate.Id;
}
```

The `Search` method will attempt to resolve invalid requests, for instance if you pass a salary filter and a salary facet the facet will be removed. When these validation changes are made the `ILoggerFactory` will log a warning.

The results will include anonymised candidates, and you can use the `Id` property to fetch the full version.

### Get a candidate

There are two methods, `Candidate` gets the same structure as search, but with contact details, `CV` gets the CV  file as a collection of `byte`:

```c#
var candidate = await stepStone.Candidate(token, "brand name", id);
var cv = await stepStone.CV(token, "brand name", id);
```

Calling either of these will reduce quota by one unless the candidate is already recently purchased.

### Exceptions

Three kinds of expected exceptions can be thrown:

- `StepStoneAuthenticationException` if thrown by `Authenticate` this means invalid credentials, otherwise it means the current token has expired.
- `StepStoneServiceException` issues with the service or gateway, generally 500 status errors.
- `StepStoneSearchException` issues with the search request object, containing the response message from the API.
