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
# workflow fails loudly instead of silently doing nothing.
published=$(curl -sf https://api.nuget.org/v3-flatcontainer/llamasharp/index.json || true)
if [ -n "$published" ]; then
  if echo "$published" | grep -Fq "\"$version\""; then
    echo "LLamaSharp $version is already published on nuget.org."
    echo "Bump <Version> in LLama/LLamaSharp.csproj before releasing again."
    exit 1
  fi
else
  echo "Warning: could not reach nuget.org to check for an existing $version, continuing anyway.";
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
