# مرحلة بناء الصورة باستخدام SDK
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /src
COPY ["ToDoApp.API/ToDoApp.API.csproj", "ToDoApp.API/"]

RUN dotnet restore "ToDoApp.API/ToDoApp.API.csproj"
COPY . .
WORKDIR "/src/ToDoApp.API"
RUN dotnet build "ToDoApp.API.csproj" -c Release -o /app/build

# مرحلة النشر (Publish)
FROM build AS publish
RUN dotnet publish "ToDoApp.API.csproj" -c Release -o /app/publish

# مرحلة النهاية (Final)
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ToDoApp.API.dll"]
