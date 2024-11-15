FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build-env
WORKDIR /MonederoTarjetaAppCommand

# Copy everything
COPY . ./

# Restore as distinct layers
#RUN dotnet restore
RUN dotnet restore MonederoTarjetaAppCommand.sln --configfile NuGet.config

# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine
WORKDIR /MonederoTarjetaAppCommand
COPY --from=build-env /MonederoTarjetaAppCommand/out .
RUN apk add --no-cache tzdata && \
    cp /usr/share/zoneinfo/America/Mexico_City /etc/localtime && \
    echo "America/Mexico_City" > /etc/timezone && \
    apk del tzdata && \
    apk add --no-cache icu-libs  

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8

ENTRYPOINT ["dotnet", "MonederoTarjetaAppCommand.Api.dll"]
