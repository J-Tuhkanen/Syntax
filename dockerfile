# Use the official ASP.NET runtime image for .NET 8
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
# EXPOSE 80
# EXPOSE 443

# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only the necessary projects
COPY ["Syntax.API/Syntax.API.csproj", "./Syntax.API/"]
COPY ["Syntax.Core/Syntax.Core.csproj", "./Syntax.Core/"]

# Restore dependencies
RUN dotnet restore "./Syntax.API/Syntax.API.csproj"

# Copy the entire project folders
COPY ./Syntax.API ./Syntax.API
COPY ./Syntax.Core ./Syntax.Core
EXPOSE 7181
EXPOSE 5146

# Set the working directory to Syntax.API and publish
WORKDIR /src/Syntax.API
RUN dotnet publish "Syntax.API.csproj" -c Release -o /app/publish

# Final stage: Use the base image to run the app
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Syntax.API.dll"]