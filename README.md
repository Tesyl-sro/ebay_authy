# Ebay Authenticator

## Build commands:
Current platform (no cross-compilation):
```sh
dotnet publish -c Release ebay_authy.csproj --self-contained
```

For Windows x64:
```sh
dotnet publish -c Release ebay_authy.csproj --self-contained -r win-x64
```

For Linux x64:
```sh
dotnet publish -c Release ebay_authy.csproj --self-contained -r linux-x64
```

Self-contained executables are in `./bin/Release/net8.0/[PLATFORM]/publish/`.