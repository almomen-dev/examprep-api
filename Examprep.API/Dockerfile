FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Examprep.API/Examprep.API.csproj", "Examprep.API/"]
COPY ["Examprep.Application/Examprep.Application.csproj", "Examprep.Application/"]
COPY ["Examprep.Domain/Examprep.Domain.csproj", "Examprep.Domain/"]
COPY ["Examprep.Infrastructure/Examprep.Infrastructure.csproj", "Examprep.Infrastructure/"]

RUN dotnet restore "Examprep.API/Examprep.API.csproj"

COPY . .
WORKDIR "/src/Examprep.API"
RUN dotnet publish "Examprep.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Examprep.API.dll"]