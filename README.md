# Wcf.Extensions.OpenIdConnect

WCF have traditionally relied on Kerberos or WS-Federation for authorization. This project contains packages which brings compatibility with the modern OAuth 2.0/OpenID Connect-world to WCF, and is heavily inspired by [Dominick Baier's blog post](https://leastprivilege.com/2015/07/02/give-your-wcf-security-architecture-a-makeover-with-identityserver3/) about placing the JWT token in a SAML assertion container.

## DL;DR

Copy your favorite way of configurating the wrapped JWT authorization service behavior from one of the provided [samples](./samples/).

## Client

### Install NuGet-packages

```
Install-Package Wcf.Extensions.OpenIdConnect.Client
```

### Make Service Call

```
// Request token
var client = new WrappedJwtClient(
    clientId: "my-client-id",
    clientSecret: "my-client-secret",
    address: "https://{my-authorization-server}/oauth2/token");
var token = await client.RequestClientCredentialsAsync();
// Invoke service method proxy
var channel = WrappedJwtChannelFactory.Create<IMyService>(token, "https://{some-service}");
channel.MyMethod();
```

The `WrappedJwtClient` use the standard `IdentityModel.Client.TokenClient` to request JWT tokens, but wraps them in a SAML assertion. For wrapping JWT tokens manually, use the static `SamlUtils.WrapJwt("...")` helper method.

For requesting a JWT token over Resource Owner Credential Grant, use the `RequestResourceOwnerPasswordAsync(...)` method of the `WrappedJwtClient`.

## Service Host

### Install NuGet-packages

```
Install-Package Wcf.Extensions.OpenIdConnect.Service
```

### Configure Authorization

Authorization is enforced through the custom service behavior, `WrappedJwtAuthorizationServiceBehavior`.

The behavior requires that all requests contain a wrapped JWT token which passes the validation logic of the standard `System.IdentityModel.Tokens.JwtSecurityTokenHandler`, that is it contains the expected audience and issuer.

The behavior gets the issuer claim and other configuration through an [OpenID Connect discovery endpoint](https://openid.net/specs/openid-connect-discovery-1_0.html), here called metadataAddress to use the same wording as the popular `Microsoft.Owin.Security.OpenIdConnect` package for MVC applications.

To authorize with the claims contained in the JWT token there is a possibility to configure service wide authorization through an expected scope. Method authorization is possible by inspecting the scopes though `ClaimsPrincipal.Current.Claims`.

The discovery endpoint metadata address and expected audience can either be specified in code or as app settings.

There are four alternatives for how to configure authorization.

#### Alternative 1) Add Behavior by App.Config or Web.Config

```
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
                                 validAudience="my-expected-audience"
                                 metadataAddress="https://{my-authorization-server}/.well-known/openid-configuration" />
      </behavior>
    </serviceBehaviors>
  </behaviors>
</system.serviceModel>
```

Or to pick up valid audience and metadata address from app settings:

```
<appSettings>
  <add key="oid:ValidAudience" value="my-expected-audience" />
  <add key="oid:MetadataAddress" value="https://{my-authorization-server}/.well-known/openid-configuration" />
</appSettings>
<system.serviceModel>
  ...
  <behaviors>
    <serviceBehaviors>
      <behavior>
        <wrappedJwtAuthorization requiredScopes="write" />
      </behavior>
    </serviceBehaviors>
  </behaviors>
</system.serviceModel>
```

#### Alternative 2) ServiceHostBase Extension Method

```
ServiceHost host = ...
var config = await OpenIdConnectConfigurationClient.RequestConfigurationAsync(
    "https://{my-authorization-server}/.well-known/openid-configuration");
host.AddWrappedJwtAuthorization(
    config, requiredScopes: "write", validAudience: "my-expected-audience");
```

#### Alternative 3) Add Behavior by Code

```
ServiceHost host = ...
host.Description.Behaviors.Add(new WrappedJwtAuthorizationServiceBehavior(
    requiredScopes: "write",
    validAudience: "my-expected-audience",
    metadataAddress: "https://{my-authorization-server}/.well-known/openid-configuration"));
```

Or to pick up valid audience and metadata address from app settings:

```
ServiceHost host = ...
host.Description.Behaviors.Add(new WrappedJwtAuthorizationServiceBehavior(requiredScopes: "write"));
```

#### Alternative 4) Add Behavior by Attribute

```
[WrappedJwtAuthorizationServiceBehavior(
    requiredScopes: "write",
    validAudience: "my-expected-audience",
    metadataAddress: "https://{my-authorization-server}/.well-known/openid-configuration")]
internal class Service : IService { ... }
```

Or to pick up valid audience and metadata address from app settings:

```
[WrappedJwtAuthorizationServiceBehavior(requiredScopes: "write)]
internal class Service : IService { ... }
```

### Required Binding

The provided wrapped JWT authorization service behavior or extension only works together with the `WS2007FederationHttpBinding`, which can be configured with code as:

```
var binding = new WS2007FederationHttpBinding(
    WSFederationHttpSecurityMode.TransportWithMessageCredential);
binding.Security.Message.EstablishSecurityContext = false;
binding.Security.Message.IssuedKeyType = SecurityKeyType.BearerKey;
```

Or with App.Config or Web.Config:

```
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

TBD: access_token_issuer
