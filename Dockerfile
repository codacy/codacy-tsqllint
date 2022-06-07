FROM mcr.microsoft.com/dotnet/sdk:6.0 AS buildimage

WORKDIR /workdir/

COPY . .
RUN dotnet publish -c Release

FROM mcr.microsoft.com/dotnet/runtime:6.0-alpine

COPY --from=buildimage /workdir/src/Analyzer/bin/Release/netcoreapp6.0/publish/*.dll \
                       /workdir/src/Analyzer/bin/Release/netcoreapp6.0/publish/Analyzer.runtimeconfig.json /opt/docker/bin/

COPY docs /docs/

RUN adduser -u 2004 -D docker
RUN chown -R docker:docker /docs

ENTRYPOINT [ "dotnet", "/opt/docker/bin/Analyzer.dll" ]
