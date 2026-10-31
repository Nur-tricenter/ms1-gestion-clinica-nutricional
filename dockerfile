# Build context is the repository root. The kernels (Nur.Tricenter.*) come from the organization's
# GitHub Packages feed, so the build needs credentials:
#   NUGET_TOKEN=<read:packages token> docker build --build-arg NUGET_USERNAME=<user> --secret id=nuget_token,env=NUGET_TOKEN --target api .
# Targets: `api` (default, the REST API / migration job) and `worker` (outbox relay).

#Stage 1 - Builder
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder
ARG NUGET_USERNAME
WORKDIR /usr/src/app
COPY . .
# The token is a BuildKit secret: it is never stored in an image layer or in the build history.
RUN --mount=type=secret,id=nuget_token \
    NUGET_USERNAME="$NUGET_USERNAME" NUGET_TOKEN="$(cat /run/secrets/nuget_token)" \
    dotnet restore GestionClinicaNutricionalService.sln
RUN dotnet publish GestionClinicaNutricionalService.WebApi -c Release --no-restore -o /publish/api
RUN dotnet publish GestionClinicaNutricional.WorkerService -c Release --no-restore -o /publish/worker

#Stage 2 - Worker runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS worker
WORKDIR /usr/share/app
COPY --from=builder /publish/worker /usr/share/app
ENTRYPOINT ["dotnet", "GestionClinicaNutricional.WorkerService.dll"]

#Stage 3 - API runtime (last, so it is the default target)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS api
WORKDIR /usr/share/app
COPY --from=builder /publish/api /usr/share/app
EXPOSE 80
ENV ASPNETCORE_URLS=http://*:80
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
ENTRYPOINT ["dotnet", "GestionClinicaNutricionalService.WebApi.dll"]
