
using BenchmarkDotNet.Running;
using OnionCSharpPoc.Benchmarks;

var summary = BenchmarkRunner.Run<MoviesBenchmark>();