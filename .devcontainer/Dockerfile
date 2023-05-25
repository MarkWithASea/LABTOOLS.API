# syntax=docker/dockerfile:1
# FROM mcr.microsoft.com/dotnet/sdk:5.0
# [Choice] .NET version: 6.0, 5.0, 3.1, 2.1
ARG VARIANT="6.0"
FROM mcr.microsoft.com/vscode/devcontainers/dotnet:${VARIANT} as development

COPY . /app
WORKDIR /app
RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]
RUN dotnet tool install dotnet-ef --tool-path /usr/local/bin
RUN curl "https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip" -o "/tmp/awscliv2.zip"
RUN unzip /tmp/awscliv2.zip -d /tmp
RUN /tmp/aws/install

FROM mcr.microsoft.com/dotnet/aspnet:${VARIANT} AS base
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT="Development"
EXPOSE 5000
ENV ASPNETCORE_URLS=http://*:5000

FROM mcr.microsoft.com/dotnet/sdk:${VARIANT} AS build
WORKDIR /src
COPY ["LABTOOLS.API.csproj", "./"]
RUN dotnet restore "LABTOOLS.API.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "LABTOOLS.API.csproj" -c Staging -o /app/build
