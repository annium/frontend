set shell := ["bash", "-cu"]
set positional-arguments
# lib.just is copied in by the umbrella repo's `just copy-ci`; recipes redefined below
# override the shared ones.
set allow-duplicate-recipes := true

import 'lib.just'

# overrides

test:
    @echo "=== $0 ==="
    dotnet test -c Release --no-build --nologo --logger "trx;LogFilePrefix=test-results.trx"

update:
    @echo "=== $0 ==="
    dotnet tool list --format json | jq -r '.data[] | "\(.packageId)"' | xargs -I% dotnet tool install %
    dotnet tool run xs update all dotnet -sc -ic

# demo

demo-blazor-ant:
    @echo "=== $0 ==="
    cd web/demo/Demo.Blazor.Ant && dotnet watch run

demo-blazor-ant-prod:
    @echo "=== $0 ==="
    cd web/demo/Demo.Blazor.Ant && rm -rf dist && dotnet publish -c Release -o dist && dotnet serve --directory dist/wwwroot -p 5004 -q

demo-blazor-interop:
    @echo "=== $0 ==="
    cd web/demo/Demo.Blazor.Interop && dotnet watch run

demo-blazor-interop-prod:
    @echo "=== $0 ==="
    cd web/demo/Demo.Blazor.Interop && rm -rf dist && dotnet publish -c Release -o dist && dotnet serve --directory dist/wwwroot -p 5002 -q

demo-blazor-charts:
    @echo "=== $0 ==="
    cd web/demo/Demo.Blazor.Charts && dotnet watch run

demo-blazor-charts-prod:
    @echo "=== $0 ==="
    cd web/demo/Demo.Blazor.Charts && rm -rf dist && dotnet publish -c Release -o dist && dotnet serve --directory dist/wwwroot -p 5003 -q
