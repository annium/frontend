setup:
	dotnet tool restore

format:
	dotnet tool run csharpier format . --config-path $(shell pwd)/.editorconfig
	dotnet tool run xs format -sc -ic

format-full: format
	dotnet format style
	dotnet format analyzers

ensure-no-changes:
	@if [[ -n "$$(git status --porcelain)" ]]; then \
		echo "Changes detected:"; \
		git status; \
		git --no-pager diff --no-color --exit-code; \
	fi

update:
	dotnet tool list --format json | jq -r '.data[] | "\(.packageId)"' | xargs -I% dotnet tool install %
	dotnet tool run xs update all dotnet -sc -ic

clean:
	dotnet tool run xs clean -sc -ic
	find . -type f -name '*.nupkg' | xargs rm

buildNumber?=0
build:
	dotnet build -c Release --nologo -p:BuildNumber=$(buildNumber)

test:
	dotnet test -c Release --no-build --nologo --logger "trx;LogFilePrefix=test-results.trx"

pack:
	dotnet pack --no-build -o . -c Release -p:SymbolPackageFormat=snupkg

publish:
	dotnet nuget push "*.nupkg" --source https://api.nuget.org/v3/index.json --api-key $(apiKey)
	find . -type f -name '*.nupkg' | xargs rm

docs-lint:
	dotnet tool run doclint lint -w . -i '**/*.cs' -e '**/obj/**/*.cs'

docs-clean:
	rm -rf _site api

docs-metadata:
	dotnet tool run docfx metadata docfx.json

docs-build:
	dotnet tool run docfx docfx.json

docs-serve:
	dotnet tool run docfx serve _site

docs-watch:
	dotnet tool run docfx docfx.json --serve


demo-blazor-ant:
	cd web/demo/Demo.Blazor.Ant && dotnet watch run

demo-blazor-ant-prod:
	cd web/demo/Demo.Blazor.Ant && rm -rf dist && dotnet publish -c Release -o dist && dotnet serve --directory dist/wwwroot -p 5004 -q

demo-blazor-interop:
	cd web/demo/Demo.Blazor.Interop && dotnet watch run

demo-blazor-interop-prod:
	cd web/demo/Demo.Blazor.Interop && rm -rf dist && dotnet publish -c Release -o dist && dotnet serve --directory dist/wwwroot -p 5002 -q

demo-blazor-charts:
	cd web/demo/Demo.Blazor.Charts && dotnet watch run

demo-blazor-charts-prod:
	cd web/demo/Demo.Blazor.Charts && rm -rf dist && dotnet publish -c Release -o dist && dotnet serve --directory dist/wwwroot -p 5003 -q

.PHONY: $(MAKECMDGOALS)
