FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Brightlux.csproj ./
RUN dotnet restore Brightlux.csproj
COPY . ./
RUN dotnet publish Brightlux.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=80
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
EXPOSE 80
COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "Brightlux.dll"]
