
// Disable running tests in parallel. Some tests load language models and try to and
// this can cause a lockup due to memory thrashing and terrible performance.
// e.g. 7GB model loaded by 10 different tests and trying to evaluate prompts
[assembly: CollectionBehavior(DisableTestParallelization = true)]
