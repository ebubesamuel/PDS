using System.Diagnostics;
using System.Threading;
namespace question1
{
    class Program
    {
        public static void Sort(int[] array, int numberOfThreads)
        {
            // Divide the array into sub-arrays based on the number of threads
            int[][] subArrays = SplitArray(array, numberOfThreads);

            // Create tasks and assign each to a sub-array
            Task[] tasks = new Task[numberOfThreads];

            for (int i = 0; i < numberOfThreads; i++)
            {
                int[] subArray = subArrays[i];
                tasks[i] = Task.Run(() => SortSubArray(subArray));
            }

            // Wait for all tasks to finish and merge the sorted sub-arrays
            Task.WaitAll(tasks);

            MergeSubArrays(array, subArrays);
        }

        // The method each task will execute
        private static void SortSubArray(int[] subArray)
        {
            // Implement bubble sort on the sub-array
            int n = subArray.Length;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (subArray[j] > subArray[j + 1])
                    {
                        int temporary = subArray[j];
                        subArray[j] = subArray[j + 1];
                        subArray[j + 1] = temporary;
                    }
                }
            }
        }

        // Method to merge sorted sub-arrays
        private static void MergeSubArrays(int[] array, int[][] subArrays)
        {
            int[] result = new int[array.Length];
            int k = 0;

            for (int i = 0; i < subArrays.Length; i++)
            {
                for (int j = 0; j < subArrays[i].Length; j++)
                {
                    result[k++] = subArrays[i][j];
                }
            }

            Array.Copy(result, array, array.Length);
        }

        public static void Main()
        {
            // Create a large random array and pass the amount the array should be
            int[] array = GenerateRandomArray(100000);

            // Call the sort method with this array and a set number of threads (2, 3, 4, 6)
            foreach (int threads in new[] { 2, 3, 4, 6 })
            {
                Console.WriteLine($"Sorting with {threads} threads...");

                // Clone the array to ensure a fresh start for each test
                int[] testArray = (int[])array.Clone();

                // Measure and display the sorting time
                Stopwatch stopwatch = Stopwatch.StartNew();
                Sort(testArray, threads);
                stopwatch.Stop();

                Console.WriteLine($"Sorting time with {threads} threads: {stopwatch.Elapsed.TotalMilliseconds} ms");
                //Console.WriteLine();

                //Console.ReadKey();
            }
            Console.WriteLine("Tasks completed");

        }

        private static int[] GenerateRandomArray(int size)
        {
            Random random = new Random();
            int[] array = new int[size];

            for (int i = 0; i < size; i++)
            {
                array[i] = random.Next();
            }

            return array;
        }

        private static int[][] SplitArray(int[] array, int numberOfPartitions)
        {
            int[][] result = new int[numberOfPartitions][];

            //Split the array into partitions
            int partSize = array.Length / numberOfPartitions;

            for (int i = 0; i < numberOfPartitions; i++)
            {
                result[i] = new int[partSize];
                Array.Copy(array, i * partSize, result[i], 0, partSize);
            }

            return result;
        }
    }
}


