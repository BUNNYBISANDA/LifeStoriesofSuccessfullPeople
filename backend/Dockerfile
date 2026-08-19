FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY SuccessLessons.Api/ SuccessLessons.Api/
RUN dotnet publish SuccessLessons.Api/SuccessLessons.Api.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_ENVIRONMENT=Production
# Avoids loading native ICU for globalization — harmless for a JSON API with
# no culture-specific formatting, and rules out one class of container issue.
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true
# The actual cause of the startup segfaults: WebApplication.CreateBuilder
# sets up a FileSystemWatcher on appsettings.json for hot-reload, which
# consumes an inotify instance. Render's container hit the OS inotify limit
# (128) — most likely from watches leaked by earlier crashed instances during
# this debugging session — which turned into a hard crash (exit 139) instead
# of a caught exception. Config hot-reload isn't needed in a container that
# gets redeployed on every change anyway, so disable the watcher outright.
ENV DOTNET_hostBuilder__reloadConfigOnChange=false
ENTRYPOINT ["dotnet", "SuccessLessons.Api.dll"]
