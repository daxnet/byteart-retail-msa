#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ByteartRetail.Services.ShoppingCarts/ByteartRetail.Services.ShoppingCarts.csproj", "ByteartRetail.Services.ShoppingCarts/"]
RUN dotnet restore "ByteartRetail.Services.ShoppingCarts/ByteartRetail.Services.ShoppingCarts.csproj"
COPY . .
WORKDIR "/src/ByteartRetail.Services.ShoppingCarts"
RUN dotnet build "ByteartRetail.Services.ShoppingCarts.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ByteartRetail.Services.ShoppingCarts.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ByteartRetail.Services.ShoppingCarts.dll"]