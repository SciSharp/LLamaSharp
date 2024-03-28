cmake_minimum_required(VERSION 3.8)
project(LLamaSharpCpp VERSION 0.1.0 LANGUAGES CXX CSharp)
if(NOT MSVC)
         message(FATAL_ERROR "This CMake file only works with MSVC.")
endif(NOT MSVC)
	
include_external_msproject(LLama.Unittest ./LLama.Unittest/LLama.Unittest.csproj GUID 9A19103F-16F7-4668-BE54-9A1E7A4F7556 TYPE BAC1CFA9-E6AC-4BD0-A548-A8066D3C467E)

include_external_msproject(LLama.Examples ./LLama.Examples/LLama.Examples.csproj GUID 9A19103F-16F7-4668-BE54-9A1E7A4F7556 TYPE BD1909AD-E1F8-476E-BC49-E394FF0470CE)

include_external_msproject(LLamaSharp ./LLama/LLamaSharp.csproj GUID 9A19103F-16F7-4668-BE54-9A1E7A4F7556 TYPE 01A12D68-DE95-425E-AEEE-2D099305036D)

include_external_msproject(LLama.WebAPI ./LLama.WebAPI/LLama.WebAPI.csproj GUID 9A19103F-16F7-4668-BE54-9A1E7A4F7556 TYPE D3CEC57A-9027-4DA4-AAAC-612A1EB50ADF)

include_external_msproject(LLama.Web ./LLama.Web/LLama.Web.csproj GUID 9A19103F-16F7-4668-BE54-9A1E7A4F7556 TYPE C3531DB2-1B2B-433C-8DE6-3541E3620DB1)

include_external_msproject(LLamaSharp.SemanticKernel ./LLama.SemanticKernel/LLamaSharp.SemanticKernel.csproj GUID 9A19103F-16F7-4668-BE54-9A1E7A4F7556 TYPE D98F93E3-B344-4F9D-86BB-FDBF6768B587)

include_external_msproject(LLamaSharp.KernelMemory ./LLama.KernelMemory/LLamaSharp.KernelMemory.csproj GUID 9A19103F-16F7-4668-BE54-9A1E7A4F7556 TYPE E5589AE7-B86F-4343-A1CC-8E5D34596E52)

add_subdirectory(./llama.cpp)