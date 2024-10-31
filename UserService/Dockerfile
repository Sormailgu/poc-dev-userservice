# Use the official .NET 8 SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY UserService/*.csproj ./UserService/
COPY UserService.Library/*.csproj ./UserService.Library/
RUN dotnet restore ./UserService/UserService.csproj

# Copy everything else and build
COPY . ./
WORKDIR /app/UserService
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/UserService/out .

# Expose ports 80 and 443
EXPOSE 80
EXPOSE 443


ENTRYPOINT ["dotnet", "UserService.dll"]