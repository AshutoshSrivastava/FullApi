FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT [ "dotnet", "PlatformService.dll" ]

# Build docker image dotn't forget '.'
# docker build -t erashu/platformservice .
# Run 
# -p means port  external:internal -d <dockerhubid>/platformservice  
# -d means detach mode
# docker run -p 8080:80 -d erashu/platformservice
# docker ps
# docker start <id>
# docker stop <id>
# docker push <dockerid>/platformservice