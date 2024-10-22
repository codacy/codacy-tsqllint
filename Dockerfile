FROM mcr.microsoft.com/dotnet/sdk:8.0 AS buildimage

WORKDIR /workdir/

COPY . .
RUN dotnet publish -c Release

FROM mcr.microsoft.com/dotnet/runtime:8.0

COPY --from=buildimage /workdir/src/Analyzer/bin/Release/netcoreapp8.0/publish/*.dll \
                       /workdir/src/Analyzer/bin/Release/netcoreapp8.0/publish/Analyzer.runtimeconfig.json /opt/docker/bin/

COPY docs /docs/

RUN adduser --uid 2004 --disabled-password --gecos "" docker
RUN chown -R docker:docker /docs

ENTRYPOINT [ "dotnet", "/opt/docker/bin/Analyzer.dll" ]
