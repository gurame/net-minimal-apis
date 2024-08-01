build:
	dotnet build MinimalApis.sln

clean:
	dotnet clean MinimalApis.sln

restore:
	dotnet restore MinimalApis.sln
	
test:
	dotnet test MinimalApis.sln

run:
	dotnet run --project src/Library.Api/Library.Api.csproj