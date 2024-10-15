using System;
using System.Collections.Generic;
using System.Text;

namespace ConditionalMatrixCounter
{
	public class Matrix
	{
		private double[,] data { get; }
		private int rows { get; }
		private int cols { get; }

		public Matrix(double[,] data)
		{
			if (data.Length == 0) throw new ArgumentException("data array cant be empty");

			rows = data.GetLength(0);
			cols = data.GetLength(1);
			if (rows == 0 || cols == 0) throw new ArgumentException("rows and cols must be greater than 0");

			this.data = data;

		}

		public static Matrix Random(int n, int m, double fillMin = 0.0, double fillMax = 1.0)
		{
			if (n <= 0 || m <= 0) throw new ArgumentException("n and m must be greater than 0");

			Random random = new();
			double[,] data = new double[n, m];
			for (int i = 0; i < n; i++)
				for (int j = 0; j < m; j++)
					data[i, j] = fillMin + random.NextDouble() * (fillMax - fillMin);
			return new Matrix(data);
		}

		public int CountSatisfyCondition(Func<double, bool> callBack)
		{
			int counter = 0;
			foreach (var number in data)
				if (callBack(number)) counter++;
			return counter;
		}

		private int CountSatisfyConditionBlocks(
			Func<double, bool> callBack,
			int currentThread, int totalThreads,
			int totalBlocksRow, int totalBlocksCol, int blockSize)
		{
			int count = 0;

			for (int blockIndex = currentThread; blockIndex < totalBlocksRow * totalBlocksCol; blockIndex += totalThreads)
			{
				int blockRow = blockIndex / totalBlocksCol;
				int blockCol = blockIndex % totalBlocksCol;

				for (int row = blockRow * blockSize; row < Math.Min((blockRow + 1) * blockSize, rows); row++)
					for (int col = blockCol * blockSize; col < Math.Min((blockCol + 1) * blockSize, cols); col++)
						if (callBack(data[row, col])) count++;
			}
			return count;
		}
		public int ThreadCountSatisfyCondition(Func<double, bool> callBack, int blockSize = 50, int? numOfThreads = null)
		{
			numOfThreads ??= Environment.ProcessorCount - 1;
			int threadCount = (int)numOfThreads;

			int totalBlocksRow = (int)Math.Ceiling((double)rows / blockSize);
			int totalBlocksCol = (int)Math.Ceiling((double)cols / blockSize);

			threadCount = Math.Min(threadCount, totalBlocksRow * totalBlocksCol);

			Thread[] threads = new Thread[threadCount];
			int[] results = new int[threadCount];

			for (int i = 0; i < threadCount; i++)
			{
				int index = i;
				threads[index] = new Thread(() =>
				{
					results[index] = CountSatisfyConditionBlocks(
						callBack, index, threadCount, totalBlocksRow, totalBlocksCol, blockSize
						);
				});
				threads[index].Start();
			}

			foreach (var thread in threads)
				thread.Join();

			int totalCount = 0;
			foreach (var count in results)
				totalCount += count;

			return totalCount;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < cols; j++)
				{
					sb.Append(data[i, j].ToString("0.00"));
					if (j < cols - 1) sb.Append("\t");
				}
				sb.AppendLine();
			}
			return sb.ToString();
		}
		public static implicit operator string(Matrix matrix) => matrix.ToString();
	}
}
