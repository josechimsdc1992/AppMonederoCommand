FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build-env
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
# RUN dotnet restore
RUN dotnet restore AppMonederoCommandService.sln --configfile NuGet.config

# Build and publish a release
RUN dotnet publish -c Release -o out
# AppMonederoCommand/AppMonederoCommand.Api.csproj

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine
WORKDIR /App
COPY --from=build-env /App/out .

RUN apk add --no-cache tzdata && \
    cp /usr/share/zoneinfo/America/Mexico_City /etc/localtime && \
    echo "America/Mexico_City" > /etc/timezone && \
    apk del tzdata && \
    apk add --no-cache icu-libs  
        
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8

ENTRYPOINT ["dotnet", "AppMonederoCommand.Api.dll"]
