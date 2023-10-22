version="$1" # version is in format "A.B.C"
is_minor="$2"    # type is "minor" or "patch"
is_patch="$3"    # type is "minor" or "patch"

echo "is_minor: $is_minor"
echo "is_patch: $is_patch"

if [ "$is_minor" = true ] && [ "$is_patch" = true ]; then
  type="minor_patch"
  echo "When both minor version and patch version are specified, the minor version will increase and patch version becomes 0."
elif [ "$is_minor" = true ] && [ "$is_patch" = false ]; then
  type="minor"
  echo "dicided to update minor version"
elif [ "$is_minor" = false ] && [ "$is_patch" = true ]; then
  type="patch"
  echo "dicided to update patch version"
else
  echo "At least one of minor version and patch version should be specified."
fi

if [[ $type == "minor" ]]; then
    regex="[0-9]+\.([0-9]+)\.[0-9]+$"
    if [[ $version =~ $regex ]]; then
        b="${BASH_REMATCH[1]}"
        b=$((b + 1))  # 将 B 值加 1
        updated_version="${version%.*}.$b.${version##*.}"  # 构建更新后的版本号
        echo "Updated version: $updated_version"
    else
        echo "Invalid version format" exit 1
    fi
elif [[ $type == "patch" ]]; then
    regex="([0-9]+)$"
    if [[ $version =~ $regex ]]; then
        c="${BASH_REMATCH[1]}"
        c=$((c + 1))  # 将 C 值加 1
        updated_version="${version%.*}.$c"  # 构建更新后的版本号
        echo "Updated version: $updated_version"
    else
        echo "Invalid version format"
        exit 1
    fi
elif [[ $type == "minor_patch" ]]; then
    regex="[0-9]+\.([0-9]+)\.[0-9]+$"
    if [[ $version =~ $regex ]]; then
        b="${BASH_REMATCH[1]}"
        b=$((b + 1))  # 将 B 值加 1
        updated_version="${version%.*}.$b.0"  # 构建更新后的版本号
        echo "Updated version: $updated_version"
    else
        echo "Invalid version format" exit 1
    fi
else
    echo "Invalid type"
    exit 1
fi

# preparation before packing backends
mkdir ./temp
mkdir ./temp/runtimes
cp ./LLama/runtimes/*.* ./temp/runtimes/
cp -r ./LLama/runtimes/build ./temp/

# pack the main package
dotnet pack ./LLama/LLamaSharp.csproj -c Release -o ./temp/ /p:PackageVersion=$updated_version /p:Version=$updated_version;
dotnet pack ./LLama.SemanticKernel/LLamaSharp.csproj -c Release -o ./temp/ /p:PackageVersion=$updated_version /p:Version=$updated_version;

# pack the backends
cd temp
nuget pack LLamaSharp.Backend.Cpu.nuspec -version $updated_version
nuget pack LLamaSharp.Backend.Cuda11.nuspec -version $updated_version
nuget pack LLamaSharp.Backend.Cuda12.nuspec -version $updated_version
nuget pack LLamaSharp.Backend.MacMetal.nuspec -version $updated_version

cd ..
exit 0