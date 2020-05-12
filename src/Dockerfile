FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
RUN curl -sL https://deb.nodesource.com/setup_13.x |  bash -
RUN apt-get install -y nodejs
WORKDIR /app
EXPOSE 80
EXPOSE 443

#build
FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
RUN curl -sL https://deb.nodesource.com/setup_14.x |  bash -
RUN apt-get install -y nodejs
WORKDIR /src
COPY ["ElectionResults.WebApi/ElectionResults.WebApi.csproj", "ElectionResults.WebApi/"]
RUN dotnet restore "ElectionResults.WebApi/ElectionResults.WebApi.csproj"
COPY . .

WORKDIR /src/ElectionResults.WebApi/ClientApp
RUN npm install
WORKDIR /src/ElectionResults.WebApi
RUN dotnet build "ElectionResults.WebApi.csproj" -c Release -o /app

#publish
FROM build AS publish
RUN dotnet publish "ElectionResults.WebApi.csproj" -c Release -o /app

FROM base AS final
ARG asp_env
ENV ASPNETCORE_ENVIRONMENT=$asp_env
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ElectionResults.WebApi.dll"]