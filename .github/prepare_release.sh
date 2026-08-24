#!/bin/bash -e

# The version to publish is read from LLama/LLamaSharp.csproj and used verbatim.
# Bump <Version> there in the release PR: whatever it says is exactly what ships.
version=$(dotnet msbuild ./LLama/LLamaSharp.csproj -getProperty:Version)
version="${version//[$'\t\r\n ']/}"

if ! [[ $version =~ ^[0-9]+\.[0-9]+\.[0-9]+$ ]]; then
  echo "Could not read a valid version from LLama/LLamaSharp.csproj (got '$version')."
  exit 1
fi

echo "Releasing version: $version";

# Refuse to rebuild a version that is already on nuget.org, so a re-run of the
# workflow fails loudly instead of silently doing nothing. This job ends in a push
# to nuget.org, so if the API cannot be reached there is no point carrying on.
nuget_index="https://api.nuget.org/v3-flatcontainer/llamasharp/index.json"
if ! published=$(curl -sf --connect-timeout 10 --max-time 60 --retry 3 --retry-delay 5 "$nuget_index"); then
  echo "Could not query nuget.org for the published versions of LLamaSharp."
  echo "Refusing to release without that check, since the release publishes to nuget.org anyway."
  exit 1
fi

if ! echo "$published" | grep -Fq '"versions"'; then
  echo "Unexpected response from $nuget_index, expected a JSON document listing versions."
  exit 1
fi

if echo "$published" | grep -Fq "\"$version\""; then
  echo "LLamaSharp $version is already published on nuget.org."
  echo "Bump <Version> in LLama/LLamaSharp.csproj before releasing again."
  exit 1
fi

mkdir ./temp;
mkdir ./temp/runtimes;
cp ./LLama/runtimes ./temp -R;
cp ./LLama/runtimes/build/*.* ./temp/;

# pack the main package
dotnet pack ./LLama/LLamaSharp.csproj -c Release -o ./temp/ /p:PackageVersion=$version /p:Version=$version /p:IncludeSymbols=true /p:SymbolPackageFormat=snupkg;

# pack the backends
cd temp
for nuspec in *.nuspec
do
  echo "Packing $nuspec"
  nuget pack $nuspec -version $version
done

# write the version to the file
echo $version > version.txt

cd ..
exit 0
