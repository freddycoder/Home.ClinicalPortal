#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV FHIR_API_URL=http://host.docker.internal:8080
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["BlazorOnFhir/BlazorOnFhir.csproj", "BlazorOnFhir/"]
RUN dotnet restore "BlazorOnFhir/BlazorOnFhir.csproj"
COPY . .
WORKDIR "/src/BlazorOnFhir"
RUN dotnet build "BlazorOnFhir.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlazorOnFhir.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /src/BlazorOnFhir/capabilities.json .
ENTRYPOINT ["dotnet", "BlazorOnFhir.dll"]