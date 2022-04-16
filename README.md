# JAP - JAIS App Packager

A CLI used to create and package JAIS apps.

## Add GitHub registry

Add the GitHub package registry to your NuGet config.

`~/.nuget/NuGet/NuGet.Config`

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
    <add key="github" value="https://nuget.pkg.github.com/carsten-computer-club/index.json" />
  </packageSources>

  <packageSourceCredentials>
      <github>
        <add key="Username" value="USERNAME" />
        <add key="ClearTextPassword" value="PAT_TOKEN" />
      </github>
    </packageSourceCredentials>
</configuration>
```

For more information check [this documentation](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry#authenticating-with-a-personal-access-token)

## Install the CLI

```bash
dotnet tool install --global Jais.App.Packager
```

## Create a new JAIS app

```bash
jap new <app name>
```

## Package an app

```bash
jap package --version 0.1.0
```