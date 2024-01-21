docker build -t mssql-2019-server .
docker run -d -p 1433:1433 --name syntax-mssql-server mssql-2019-server
REM docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=P' -p 1433:1433 --name sqlserver -v sqlvolume:/var/opt/mssql mcr.microsoft.com/mssql/server:2019-latest

pause