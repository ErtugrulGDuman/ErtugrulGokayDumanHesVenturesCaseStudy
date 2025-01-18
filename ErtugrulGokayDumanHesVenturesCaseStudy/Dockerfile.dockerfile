FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TrackingApi.csproj", "./"]
RUN dotnet restore "TrackingApi.csproj"
COPY . .
RUN dotnet build "TrackingApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TrackingApi.csproj" -c Release -o /app/publish

# Chrome ve ChromeDriver kurulumu için
FROM base AS final
RUN apt-get update && apt-get install -y \
    wget \
    gnupg2 \
    unzip

# Chrome kurulumu
RUN wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add - \
    && echo "deb http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google.list \
    && apt-get update \
    && apt-get install -y google-chrome-stable

# ChromeDriver kurulumu
RUN CHROME_VERSION=$(google-chrome --version | cut -d ' ' -f 3 | cut -d '.' -f 1) \
    && CHROMEDRIVER_VERSION=$(curl -s "https://chromedriver.storage.googleapis.com/LATEST_RELEASE_$CHROME_VERSION") \
    && wget -q "https://chromedriver.storage.googleapis.com/$CHROMEDRIVER_VERSION/chromedriver_linux64.zip" \
    && unzip chromedriver_linux64.zip -d /usr/local/bin/ \
    && rm chromedriver_linux64.zip

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrackingApi.dll"]