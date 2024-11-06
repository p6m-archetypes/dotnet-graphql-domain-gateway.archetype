FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

EXPOSE 8080
EXPOSE 8081

WORKDIR /app
COPY ./publish .
# Set the entry point to the app's executable
ENTRYPOINT ["dotnet", "{{ ProjectName }}.Server.dll"]

