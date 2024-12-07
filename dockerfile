FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy project file and restore as distinct layers
COPY ["Syntax.API/Syntax.API.csproj", "./Syntax.API/"]
COPY ["Syntax.Core/Syntax.Core.csproj", "./Syntax.Core/"]

RUN dotnet restore "./Syntax.API/Syntax.API.csproj"

# Copy source code and publish app
COPY ./Syntax.API ./Syntax.API
COPY ./Syntax.Core ./Syntax.Core

WORKDIR /source/Syntax.API
RUN dotnet publish "Syntax.API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 8080
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "./Syntax.API.dll"]
