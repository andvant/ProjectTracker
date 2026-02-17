FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /app

COPY *.slnx .
COPY Directory.*.props .
COPY src/Domain/Domain.csproj src/Domain/
COPY src/Application/Application.csproj src/Application/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/
COPY src/Api/Api.csproj src/Api/
RUN dotnet restore src/Api/Api.csproj

COPY . .

WORKDIR src/Api

EXPOSE 80

CMD ["dotnet", "watch", "run", "--no-launch-profile"]
