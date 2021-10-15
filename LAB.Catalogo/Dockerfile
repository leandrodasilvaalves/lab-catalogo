FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV ConnectionStrings__DefaultConnection=null
ENV RedisConfig__InstanceName=null
ENV RedisConfig__Configuration=null
ENV RedisConfig__AbsoluteExpirationRelativeToNowFromSeconds=null
ENV RedisConfig__SlidingExpirationFromSeconds=null


FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["./LAB.Catalogo.csproj", "LAB.Catalogo.csproj"]
RUN dotnet restore "LAB.Catalogo.csproj"
COPY . .
RUN dotnet build "LAB.Catalogo.csproj" -c Release -o /build

FROM build AS publish
RUN dotnet publish "LAB.Catalogo.csproj" -c Release -o /publish

FROM base AS final
WORKDIR /app
COPY --from=publish /publish .
ENTRYPOINT [ "dotnet", "LAB.Catalogo.dll" ]