using System.Diagnostics;

namespace ConditionalMatrixCounter
{
	internal class Program
	{
		static void Main(string[] args) 
		{

            static bool condition(double num) => num > 0.5;
            SpeedTest(1000, 1000, condition, Environment.ProcessorCount - 1, 50, 10);
			SpeedTest(1000, 1000, condition, 4, 50, 10);
			SpeedTest(20000, 20000, condition, Environment.ProcessorCount - 1, 1000, 5);
			SpeedTest(20000, 20000, condition, 4, 1000, 5);
		}
		static void SpeedTest(int n, int m, Func<double, bool> condition, int? threadCount, int blockSize, int numberOfTests)
		{
			Console.WriteLine($"Test for matrix {n}x{m}, with {threadCount} threads and block size {blockSize}x{blockSize}:");
			Matrix matrix = Matrix.Random(n, m);
			Stopwatch stopwatch = new Stopwatch();

			double singleThreadedTime = 0;
			for (int i = 0; i < numberOfTests; i++)
            {
				stopwatch.Restart();
				int count = matrix.CountSatisfyCondition(condition);
				stopwatch.Stop();
				singleThreadedTime += stopwatch.ElapsedMilliseconds;
			}
			singleThreadedTime /= numberOfTests;
			Console.WriteLine($"Single-threaded average execution time: {singleThreadedTime} ms");


			double multiThreadedTime = 0;
			for (int i = 0; i < numberOfTests; i++)
			{
				stopwatch.Restart();
				int countMultithreaded = matrix.ThreadCountSatisfyCondition(condition, blockSize, threadCount);
				stopwatch.Stop();
				multiThreadedTime += stopwatch.ElapsedMilliseconds;
			}
			multiThreadedTime /= numberOfTests;
			Console.WriteLine($"Multi-threaded average execution time: {multiThreadedTime} ms");


			double speedup = (double)singleThreadedTime / multiThreadedTime;
			Console.WriteLine($"Speedup: {speedup:F2}x\n");
		}
	}
}
