FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY SuccessLessons.Api/ SuccessLessons.Api/
RUN dotnet publish SuccessLessons.Api/SuccessLessons.Api.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_ENVIRONMENT=Production
# Avoids loading native ICU for globalization — a known cause of segfaults
# in syscall-restricted containers, and unneeded for a JSON API with no
# culture-specific formatting.
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true
ENTRYPOINT ["dotnet", "SuccessLessons.Api.dll"]
