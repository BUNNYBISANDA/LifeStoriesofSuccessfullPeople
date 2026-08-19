FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY SuccessLessons.Api/SuccessLessons.Api.csproj SuccessLessons.Api/
RUN dotnet restore SuccessLessons.Api/SuccessLessons.Api.csproj

COPY SuccessLessons.Api/ SuccessLessons.Api/
RUN dotnet publish SuccessLessons.Api/SuccessLessons.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "SuccessLessons.Api.dll"]
