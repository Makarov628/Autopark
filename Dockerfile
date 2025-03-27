FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build-env
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
ENV ASPNETCORE_ENVIRONMENT=Release
RUN curl -sL https://deb.nodesource.com/setup_20.x | bash -
RUN apt-get install -y nodejs
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Release
WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "Autopark.Web.dll"]
EXPOSE 8080