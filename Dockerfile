# Use the official Microsoft SQL Server image
FROM mcr.microsoft.com/mssql/server:2019-latest

# Set required environment variables
ENV ACCEPT_EULA=Y
ENV SA_PASSWORD=Pwd12345!

# Expose the default SQL Server port
EXPOSE 1433

# Healthcheck to ensure SQL Server is running
HEALTHCHECK --interval=10s \
            --timeout=3s \
            --start-period=10s \
            --retries=3 \
            CMD /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "Pwd12345!" -Q "SELECT 1" || exit 1

# Run SQL Server process
CMD [ "/opt/mssql/bin/sqlservr" ]
