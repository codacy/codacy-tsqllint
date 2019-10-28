FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS buildimage

WORKDIR /workdir/

COPY . .
RUN dotnet publish -c Release

FROM mcr.microsoft.com/dotnet/core/runtime:2.2-alpine

COPY --from=buildimage /workdir/src/Analyzer/bin/Release/netcoreapp2.2/publish/*.dll \
              /workdir/src/Analyzer/bin/Release/netcoreapp2.2/publish/Analyzer.runtimeconfig.json /opt/docker/bin/

COPY docs /docs/

RUN adduser -u 2004 -D docker
RUN chown -R docker:docker /docs

ENTRYPOINT [ "dotnet", "/opt/docker/bin/Analyzer.dll" ]
