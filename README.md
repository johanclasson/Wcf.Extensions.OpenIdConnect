# Wcf.Extensions.OpenIdConnect

WCF have traditionally relied on Kerberos or WS-Federation for authorization. This project contains packages which bring compatibility with the modern OAuth 2.0/OpenID Connect-world to WCF, and is heavily inspired by [Dominick Baier's blog post](https://leastprivilege.com/2015/07/02/give-your-wcf-security-architecture-a-makeover-with-identityserver3/) about placing the JWT token in a SAML assertion container.

## TL;DR

Copy your favorite way of configurating the client and service from one of the provided [samples](./samples/).

## Client

### Install NuGet-packages

```powershell
Install-Package Wcf.Extensions.OpenIdConnect.Client -Prerelease
```

### Make Service Call

```csharp
// Request token
string jwt = ...
// Create channel
var channel = WrappedJwtChannelFactory.Create<IMyService>(jwt, "https://{some-service}");
// Invoke service method proxy
channel.MyMethod();
```

For example, the JWT can be retrieved with the `TokenClient` of `IdentityModel`:

```csharp
using (var client = new TokenClient(
    "https://{my-authorization-server}/oauth2/token", "my-client-id", "my-client-secret"))
{
    var token = await client.RequestClientCredentialsAsync("write");
    string jwt = token.AccessToken;
    ...
}
```

Or retrieved with the `AuthenticationContext` of `Microsoft.IdentityModel.Clients.ActiveDirectory`:

```csharp
var authContext = new AuthenticationContext("https://{my-authorization-server}/oauth2/token");
var creds = new ClientCredential("my-client-id", "my-client-secret");
var token = await authContext.AcquireTokenAsync("my-resource-id", creds);
string jwt = token.AccessToken;
...
```

## Service Host

### Install NuGet-packages

```powershell
Install-Package Wcf.Extensions.OpenIdConnect.Service -Prerelease
```

### Configure Authorization

Authorization is enforced through the custom service behavior, `WrappedJwtAuthorizationServiceBehavior`.

The behavior requires that all requests contain a wrapped JWT token which passes the validation logic of the standard `System.IdentityModel.Tokens.JwtSecurityTokenHandler`. To be more precise the tokens need to be encrypted with the expected certificate, and contain the expected audience and issuer.

The behavior gets the issuer claim and other configuration through an [OpenID Connect discovery endpoint](https://openid.net/specs/openid-connect-discovery-1_0.html), here called metadataAddress to use the same wording as the popular `Microsoft.Owin.Security.OpenIdConnect` package for MVC applications.

Additional service-wide authorization can be configured by setting expected scopes or/and roles. When using expected scopes and roles, only requests with `scp` and `roles` claims containing the expected values will be authorized. Role and scope authorization is entirely optional and can be left out if desired.

Method authorization is possible by inspecting the claims through `ClaimsPrincipal.Current.Claims`.


The discovery endpoint metadata address and expected audience can either be specified in code or as app settings.

There are four alternatives for how to configure authorization.

#### Alternative 1) Add Behavior by App.Config or Web.Config

```xml
<system.serviceModel>
  <extensions>
    <behaviorExtensions>
      <add name="wrappedJwtAuthorization"
           type="Wcf.Extensions.OpenIdConnect.Service.WrappedJwtAuthorizationExtensionElement, Wcf.Extensions.OpenIdConnect.Service, Version=1.0.0.0, Culture=neutral" />
    </behaviorExtensions>
  </extensions>
  ...
  <behaviors>
    <serviceBehaviors>
      <behavior>
        <wrappedJwtAuthorization requiredScopes="write"
                                 requiredRoles="admin"
                                 validAudience="my-expected-audience"
                                 metadataAddress="https://{my-authorization-server}/.well-known/openid-configuration" />
      </behavior>
    </serviceBehaviors>
  </behaviors>
</system.serviceModel>
```

Or to pick up valid audience and metadata address from app settings:

```xml
<appSettings>
  <add key="oid:ValidAudience" value="my-expected-audience" />
  <add key="oid:MetadataAddress" value="https://{my-authorization-server}/.well-known/openid-configuration" />
</appSettings>
<system.serviceModel>
  ...
  <behaviors>
    <serviceBehaviors>
      <behavior>
        <wrappedJwtAuthorization requiredScopes="write" requiredRoles="admin" />
      </behavior>
    </serviceBehaviors>
  </behaviors>
</system.serviceModel>
```

#### Alternative 2) ServiceHostBase Extension Method

```csharp
ServiceHost host = ...
var config = await OpenIdConnectConfigurationClient.RequestConfigurationAsync(
    "https://{my-authorization-server}/.well-known/openid-configuration");
host.AddWrappedJwtAuthorization(config,
    requiredScopes: "write", requiredRoles: "admin", validAudience: "my-expected-audience");
```

#### Alternative 3) Add Behavior by Code

```csharp
ServiceHost host = ...
host.Description.Behaviors.Add(new WrappedJwtAuthorizationServiceBehavior(
    requiredScopes: "write",
    requiredRoles: "admin",
    validAudience: "my-expected-audience",
    metadataAddress: "https://{my-authorization-server}/.well-known/openid-configuration"));
```

Or to pick up valid audience and metadata address from app settings:

```csharp
ServiceHost host = ...
host.Description.Behaviors.Add(new WrappedJwtAuthorizationServiceBehavior(
    requiredScopes: "write", requiredRoles: "admin"));
```

#### Alternative 4) Add Behavior by Attribute

```csharp
[WrappedJwtAuthorizationServiceBehavior(
    requiredScopes: "write",
    requiredRoles: "admin",
    validAudience: "my-expected-audience",
    metadataAddress: "https://{my-authorization-server}/.well-known/openid-configuration")]
internal class Service : IService { ... }
```

Or to pick up valid audience and metadata address from app settings:

```csharp
[WrappedJwtAuthorizationServiceBehavior(requiredScopes: "write", requiredRoles: "admin")]
internal class Service : IService { ... }
```

### Required Binding

The wrapped JWT authorization service behavior or extension only works together with the `WS2007FederationHttpBinding`. For example, it can be configured with code as:

```csharp
var binding = new WS2007FederationHttpBinding(
    WSFederationHttpSecurityMode.TransportWithMessageCredential);
binding.Security.Message.EstablishSecurityContext = false;
binding.Security.Message.IssuedKeyType = SecurityKeyType.BearerKey;
```

Or in App.Config or Web.Config:

```xml
<system.serviceModel>
  <bindings>
    <ws2007FederationHttpBinding>
      <binding>
        <security mode="TransportWithMessageCredential">
          <message establishSecurityContext="false" issuedKeyType="BearerKey" />
        </security>
      </binding>
    </ws2007FederationHttpBinding>
  </bindings>
</system.serviceModel>
```

### ADFS Compatibility

ADFS uses a non standard value for the `iss` claim in access tokens. OpenID Connect states [(ref.)](https://openid.net/specs/openid-connect-discovery-1_0.html#ProviderMetadata) that the issuer should be identical to the `issuer` field which is present in the metadata at the OpenID discovery endpoint. ADFS uses `access_token_issuer` instead.

The value of `access_token_issuer` is taken from the Federation Service property `Identifier` which is automatically set during installation. _(One might guess that it would be taken from the Federation Service property `IdTokenIssuer`, but this is not the case.)_

The Federation Service `Identifier` can be changed with the PowerShell command `Set-AdfsProperties -Identifier https://{my-authorization-server}`, but that might not be something that is an option in an existing federation setup because of relying parties and upstream identity providers.

The wrapped JWT authorization service behavior handles this by using the `access_token_issuer` field if it exists. And if the field does not exist, it falls back to the standard `issuer` field. That way it is compatible with both ADFS and other products, such as Azure AD.
