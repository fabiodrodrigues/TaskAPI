#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TaskAPI.sln", "."]
COPY ["TaskAPI.API/TaskAPI.API.csproj", "TaskAPI.API/"]
COPY ["TaskAPI.Application/TaskAPI.Application.csproj", "TaskAPI.Application/"]
COPY ["TaskAPI.Data/TaskAPI.Data.csproj", "TaskAPI.Data/"]
COPY ["TaskAPI.Domain/TaskAPI.Domain.csproj", "TaskAPI.Domain/"]
RUN dotnet restore "TaskAPI.API/TaskAPI.API.csproj"
COPY . .
WORKDIR "/src/TaskAPI.API"
RUN dotnet build "TaskAPI.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaskAPI.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskAPI.API.dll"]