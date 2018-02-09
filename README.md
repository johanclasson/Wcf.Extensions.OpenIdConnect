# Wcf.Extensions.OpenIdConnect

WCF have traditionally relied on Kerberos or WS-Federation for autorization. This project contains packages which brings compatibility with the modern OAuth 2.0/OpenID Connect-world to WCF, and is heavily inspired by [Dominick Baier's blog post](https://leastprivilege.com/2015/07/02/give-your-wcf-security-architecture-a-makeover-with-identityserver3/) about placing the JWT token in a SAML assertion container.

## Client

### Install NuGet-packages

```
Install-Package Wcf.Extensions.OpenIdConnect.Client
```

### Make Service Call

```
// Request token
var client = new WrappedJwtTokenClient(
    clientId: "my-client-id",
    clientSecret: "my-client-secret",
    address: "https://{my-authorization-server}/oauth2/token");
var token = await client.RequestClientCredentialsAsync();
// Invoke service method proxy
var channel = WrappedJwtChannelFactory.Create<IMyService>(token, "https://{some-service}");
channel.MyMethod();
```

The `WrappedJwtTokenClient` use the standard `IdentityModel.Client.TokenClient` to request JWT tokens, but wraps them in a SAML assertion. For wrapping JWT tokens manually, use the static `SamlUtils.WrapJwt("...")` helper method.

For requesting a JWT token over Resource Owner Credential Grant, use the `RequestResourceOwnerPasswordAsync(...)` method of the `WrappedJwtTokenClient`.

## Service Host

### Install NuGet-packages

```
Install-Package Wcf.Extensions.OpenIdConnect.Service
```

### Configure Authorization


