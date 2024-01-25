# Use the ASP.NET 6.0 image as the base image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Use the .NET 6.0 SDK image to build the project
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy the CSPROJ file and restore any dependencies (via NuGet)
COPY ["Syntax.Core/Syntax.Core.csproj", "Syntax.Core/"]
COPY ["Syntax.Tests/Syntax.Tests.csproj", "Syntax.Tests/"]
RUN dotnet restore "Syntax.Core/Syntax.Core.csproj"
RUN dotnet restore "Syntax.Tests/Syntax.Tests.csproj"

# Copy the project files and build the release
COPY Syntax.Core/ Syntax.Core/
COPY Syntax.Tests/ Syntax.Tests/

FROM build AS publish

# Build the core project
WORKDIR "/src/Syntax.Core"
RUN dotnet build "Syntax.Core.csproj" -c Release -o /app/build
RUN dotnet publish "Syntax.Core.csproj" -c Release -o /app/publish

# Build the test project and run tests
WORKDIR "/src/Syntax.Tests"
RUN dotnet build "Syntax.Tests.csproj" -c Release -o /app/build
RUN dotnet publish "Syntax.Tests.csproj" -c Release -o /app/publish
RUN dotnet test "Syntax.Tests.csproj" --configuration Release --no-build

# Generate the runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Syntax.Core.dll"]
ENTRYPOINT ["dotnet", "Syntax.Tests.dll"]

