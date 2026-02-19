FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /app
EXPOSE 80

COPY *.slnx .
COPY Directory.*.props .
COPY src/Domain/Domain.csproj src/Domain/
COPY src/Application/Application.csproj src/Application/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/
COPY src/ServiceDefaults/ServiceDefaults.csproj src/ServiceDefaults/
COPY src/Api/Api.csproj src/Api/
RUN dotnet restore src/Api/Api.csproj

COPY . .

WORKDIR src/Api

CMD ["dotnet", "run", "--no-launch-profile"]
