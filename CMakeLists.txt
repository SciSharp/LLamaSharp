#----------------------------------------------------------------------------------------
# INFO: 
# - How to use: change the flags in the 'Set one of these ON and all others OFF' section
# - CUDA: it will use automatically the CUDA SDK version installed
#
#----------------------------------------------------------------------------------------
cmake_minimum_required(VERSION 3.8)
project(LLamaSharpCpp VERSION 0.10.0 LANGUAGES CXX CSharp)
if(NOT MSVC)
         message(FATAL_ERROR "This CMake file only works with MSVC.")
endif(NOT MSVC)

#--------- Set one of these ON and all others OFF ------------------->
option(LLAMA_CUDA_AVX2           "CUDA + AVX2"          ON)
option(LLAMA_AVX2                "AVX2 (no CUDA)"       OFF)
option(LLAMA_CUDA                "CUDA (no AVX)"        OFF)
#etc... add other setups
#<--------- Set one of these ON and all others OFF -------------------

# --------------- Don't change below this line -----------------------

# Variable Settings
if(LLAMA_CUDA_AVX2)
	option(LLAMA_AVX                    "llama: enable AVX"                                		ON)
	option(LLAMA_AVX2                   "llama: enable AVX2"                               		ON)
	option(LLAMA_CUBLAS                 "llama: use CUDA"                                  		ON)
elseif(LLAMA_AVX2)
	option(LLAMA_AVX                    "llama: enable AVX"                                		ON)
	option(LLAMA_AVX2                   "llama: enable AVX2"                               		ON)
	option(LLAMA_CUBLAS                 "llama: use CUDA"                                  		OFF)
elseif(LLAMA_CUDA)
	option(LLAMA_AVX                    "llama: enable AVX"                                		OFF)
	option(LLAMA_AVX2                   "llama: enable AVX2"                               		OFF)
	option(LLAMA_CUBLAS                 "llama: use CUDA"                                  		ON)	
elseif(OTHER_SETUPS)
	#etc...
endif()

# Fixed Settings
# general
option(BUILD_SHARED_LIBS                "build shared libraries"                                ON)
option(LLAMA_STATIC                     "llama: static link libraries"                          OFF)
option(LLAMA_NATIVE                     "llama: enable -march=native flag"                      OFF)
option(LLAMA_LTO                        "llama: enable link time optimization"                  OFF)
option(LLAMA_CCACHE                     "llama: use ccache if available"                        ON)

# debug
option(LLAMA_ALL_WARNINGS               "llama: enable all compiler warnings"                   ON)
option(LLAMA_ALL_WARNINGS_3RD_PARTY     "llama: enable all compiler warnings in 3rd party libs" OFF)
option(LLAMA_GPROF                      "llama: enable gprof"                                   OFF)

# build
option(LLAMA_FATAL_WARNINGS             "llama: enable -Werror flag"                            OFF)

# sanitizers
option(LLAMA_SANITIZE_THREAD            "llama: enable thread sanitizer"                        OFF)
option(LLAMA_SANITIZE_ADDRESS           "llama: enable address sanitizer"                       OFF)
option(LLAMA_SANITIZE_UNDEFINED         "llama: enable undefined sanitizer"                     OFF)

option(LLAMA_AVX512                          "llama: enable AVX512"                             OFF)
option(LLAMA_AVX512_VBMI                     "llama: enable AVX512-VBMI"                        OFF)
option(LLAMA_AVX512_VNNI                     "llama: enable AVX512-VNNI"                        OFF)
option(LLAMA_FMA                             "llama: enable FMA"                                OFF)
# in MSVC F16C is implied with AVX2/AVX512
if (NOT MSVC)
    option(LLAMA_F16C                        "llama: enable F16C"                               OFF)
endif()

if (WIN32)
    set(LLAMA_WIN_VER "0x602" CACHE STRING 	  "llama: Windows Version")
endif()

# 3rd party libs
option(LLAMA_ACCELERATE                      "llama: enable Accelerate framework"               ON)
option(LLAMA_BLAS                            "llama: use BLAS"                                  OFF)
set(LLAMA_BLAS_VENDOR "Generic" CACHE STRING "llama: BLAS library vendor")
#option(LLAMA_CUDA_CUBLAS                     "llama: use cuBLAS for prompt processing"          OFF)
option(LLAMA_CUDA_FORCE_DMMV                 "llama: use dmmv instead of mmvq CUDA kernels"     OFF)
option(LLAMA_CUDA_FORCE_MMQ                  "llama: use mmq kernels instead of cuBLAS"         OFF)
set(LLAMA_CUDA_DMMV_X      "32" CACHE STRING "llama: x stride for dmmv CUDA kernels")
set(LLAMA_CUDA_MMV_Y        "1" CACHE STRING "llama: y block size for mmv CUDA kernels")
option(LLAMA_CUDA_F16                        "llama: use 16 bit floats for some calculations"   OFF)
set(LLAMA_CUDA_KQUANTS_ITER "2" CACHE STRING "llama: iters./thread per block for Q2_K/Q6_K")
set(LLAMA_CUDA_PEER_MAX_BATCH_SIZE "128" CACHE STRING
                                             "llama: max. batch size for using peer access")
option(LLAMA_HIPBLAS                         "llama: use hipBLAS"                               OFF)
option(LLAMA_HIP_UMA                         "llama: use HIP unified memory architecture"       OFF)
option(LLAMA_CLBLAST                         "llama: use CLBlast"                               OFF)
option(LLAMA_VULKAN                          "llama: use Vulkan"                                OFF)
option(LLAMA_VULKAN_CHECK_RESULTS            "llama: run Vulkan op checks"                      OFF)
option(LLAMA_VULKAN_DEBUG                    "llama: enable Vulkan debug output"                OFF)
option(LLAMA_VULKAN_VALIDATE                 "llama: enable Vulkan validation"                  OFF)
option(LLAMA_VULKAN_RUN_TESTS                "llama: run Vulkan tests"                          OFF)
option(LLAMA_METAL                           "llama: use Metal"                                 ${LLAMA_METAL_DEFAULT})
option(LLAMA_METAL_NDEBUG                    "llama: disable Metal debugging"                   OFF)
option(LLAMA_METAL_SHADER_DEBUG              "llama: compile Metal with -fno-fast-math"         OFF)
option(LLAMA_METAL_EMBED_LIBRARY             "llama: embed Metal library"                       OFF)
option(LLAMA_KOMPUTE                         "llama: use Kompute"                               OFF)
option(LLAMA_MPI                             "llama: use MPI"                                   OFF)
option(LLAMA_QKK_64                          "llama: use super-block size of 64 for k-quants"   OFF)
option(LLAMA_SYCL                            "llama: use SYCL"                                  OFF)
option(LLAMA_SYCL_F16                        "llama: use 16 bit floats for sycl calculations"   OFF)
option(LLAMA_CPU_HBM                         "llama: use memkind for CPU HBM"                   OFF)

option(LLAMA_BUILD_TESTS                     "llama: build tests"    							OFF)
option(LLAMA_BUILD_EXAMPLES                  "llama: build examples" 							ON)
option(LLAMA_BUILD_SERVER                    "llama: build server example"                      OFF)

# add perf arguments
option(LLAMA_PERF                            "llama: enable perf"                               OFF)

include_external_msproject(LLama.Unittest ./LLama.Unittest/LLama.Unittest.csproj GUID 9A19103F-16F7-4668-BE54-9A1E7A4F7556 TYPE BAC1CFA9-E6AC-4BD0-A548-A8066D3C467E)

include_external_msproject(LLama.Examples ./LLama.Examples/LLama.Examples.csproj GUID 9A19103F-16F7-4668-BE54-9A1E7A4F7556 TYPE BD1909AD-E1F8-476E-BC49-E394FF0470CE)

include_external_msproject(LLamaSharp ./LLama/LLamaSharp.csproj GUID 9A19103F-16F7-4668-BE54-9A1E7A4F7556 TYPE 01A12D68-DE95-425E-AEEE-2D099305036D)

include_external_msproject(LLama.WebAPI ./LLama.WebAPI/LLama.WebAPI.csproj GUID 9A19103F-16F7-4668-BE54-9A1E7A4F7556 TYPE D3CEC57A-9027-4DA4-AAAC-612A1EB50ADF)

include_external_msproject(LLama.Web ./LLama.Web/LLama.Web.csproj GUID 9A19103F-16F7-4668-BE54-9A1E7A4F7556 TYPE C3531DB2-1B2B-433C-8DE6-3541E3620DB1)

include_external_msproject(LLamaSharp.SemanticKernel ./LLama.SemanticKernel/LLamaSharp.SemanticKernel.csproj GUID 9A19103F-16F7-4668-BE54-9A1E7A4F7556 TYPE D98F93E3-B344-4F9D-86BB-FDBF6768B587)

include_external_msproject(LLamaSharp.KernelMemory ./LLama.KernelMemory/LLamaSharp.KernelMemory.csproj GUID 9A19103F-16F7-4668-BE54-9A1E7A4F7556 TYPE E5589AE7-B86F-4343-A1CC-8E5D34596E52)

add_subdirectory(./llama.cpp)