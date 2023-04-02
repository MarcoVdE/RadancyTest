FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
# Not binding https port as need to set up cert, add https://+:443
ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RadancyBankingSystemTest.Api/RadancyBankingSystemTest.Api.csproj", "RadancyBankingSystemTest.Api/"]
COPY ["RadancyBankingSystemTest.Domain/RadancyBankingSystemTest.Domain.csproj", "RadancyBankingSystemTest.Domain/"]
COPY ["RadancyBankingSystemTest.Domain.Dto/RadancyBankingSystemTest.Domain.Dto.csproj", "RadancyBankingSystemTest.Domain.Dto/"]
RUN dotnet restore "RadancyBankingSystemTest.Api/RadancyBankingSystemTest.Api.csproj"
COPY . .
WORKDIR "/src/RadancyBankingSystemTest.Api"
RUN dotnet build "RadancyBankingSystemTest.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RadancyBankingSystemTest.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RadancyBankingSystemTest.Api.dll"]
