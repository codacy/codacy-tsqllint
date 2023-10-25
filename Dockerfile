FROM mcr.microsoft.com/dotnet/aspnet:7.0.13-alpine3.18-amd64 AS buildimage

WORKDIR /workdir/

COPY . .
RUN dotnet publish -c Release

FROM mcr.microsoft.com/dotnet/runtime:7.0.13-alpine3.18-amd64

COPY --from=buildimage /workdir/src/Analyzer/bin/Release/net7.0/publish/*.dll \
                       /workdir/src/Analyzer/bin/Release/net7.0/publish/Analyzer.runtimeconfig.json /opt/docker/bin/

COPY docs /docs/

RUN adduser -u 2004 -D docker
RUN chown -R docker:docker /docs

ENTRYPOINT [ "dotnet", "/opt/docker/bin/Analyzer.dll" ]
