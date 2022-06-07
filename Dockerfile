FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS buildimage

WORKDIR /workdir/

COPY . .
RUN dotnet publish -c Release

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-alpine

COPY --from=buildimage /workdir/src/Analyzer/bin/Release/netcoreapp6.0/publish/*.dll \
                       /workdir/src/Analyzer/bin/Release/netcoreapp6.0/publish/Analyzer.runtimeconfig.json /opt/docker/bin/

COPY docs /docs/

RUN adduser -u 2004 -D docker
RUN chown -R docker:docker /docs

ENTRYPOINT [ "dotnet", "/opt/docker/bin/Analyzer.dll" ]
