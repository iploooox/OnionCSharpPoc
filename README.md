# Performance Testing with K6 and BenchmarkDotNet
This repository contains a set of performance tests for the MoviesService.
The tests are written in C# using the BenchmarkDotNet and K6 frameworks.

## Setup
To run the tests, you will need to have the following installed:

- .NET 7 SDK
- K6

## Running the Tests
### BenchmarkDotNet
To run the BenchmarkDotNet tests, open a command prompt in the Benchmark folder and run the following command:
``` bash
dotnet run -c Release
```

This will compile and run the performance tests, which will output the results to the console.

## K6
To run the K6 tests, open a command prompt in the OnionCSharpPoc folder and run the following commands:

```bash
dotnet run -c Release
```
This will start ASP.NET server.

Followed by each k6 test:

```bash
k6 run movies_add_error.js
k6 run movies_add_result_error.js
k6 run movies_add_async_result_error.js
```

## Results
### BenchmarkDotNet
The following table shows the results of the BenchmarkDotNet tests:

| Method        | Count | Mean       | StdDev   	      | Allocated|
|	        ---:|---	|---:	     |---:	              |---:	     |
| Add           | 1000  | 197.5 ms   | 55.75 ms           | 15.1 MB  |
| AddWithResult | 1000  | 148.5 ms   | 54.32 ms           | 13.09 MB |
| AddResultAsync| 1000  | 155.6 ms   | 60.12 ms           | 13.27 MB |
| Add           | 2500  | 832.7 ms   | 447.50 ms          | 35.86 MB |
| AddWithResult | 2500  | 735.9 ms   | 735.9 ms           | 32.72 MB |
| AddResultAsync| 2500  | 714.5 ms   | 714.5 ms           | 33.2 MB  |
| Add           | 5000  | 3,929.8 ms | 3,360.42 ms        | 35.86 MB |
| AddWithResult | 5000  | 3,545.4 ms | 3,160.95 ms        | 32.72 MB |
| AddResultAsync| 5000  | 4,188.6 ms | 3,770.92 ms        | 33.2 MB  |

### K6
The following table shows the results of the K6 load tests all ran with 10 VUs:

| Method        | Requests/sec | Avg. Latency (ms)   	| Min Latency (ms)   	| Max Latency (ms)  	| 95th Percentile (ms)  	 |
|---:	        |---:	          |---:	|---:	|---:	|---:   |
| Add           | 9406/s  | 1.04ms   | 45µs | 28.72ms | 4.17ms |
| AddWithResult | 15972/s | 605.34µs | 39µs | 34.85ms | 2.46ms |
| AddResultAsync| 20505/s | 467.77µs | 35µs | 34.24ms | 1.49ms |

![image](https://user-images.githubusercontent.com/115003487/230913806-5fdd7cee-53a5-426f-9959-49b5a445ec94.png)


## Conclusion
Based on the results, we can see that all three methods can handle a high volume of requests. However, the K6 tests show that the AddResultAsync method performs the best in terms of requests per second and latency.

It's important to note that the results may vary based on the hardware and network configurations used to run the tests.

## License
This code is licensed under the MIT License. See LICENSE for details.
