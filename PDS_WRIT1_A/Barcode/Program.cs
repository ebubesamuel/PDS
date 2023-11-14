
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Barcode;

public class Tool
{
    // Properties to store tool type and barcode
    public int type { get; set; }
    public int barcode { get; set; }

    // Constructor to initialize a Tool object with a type and a barcode
    public Tool(int _type, int _barcode)
    {
        type = _type;
        barcode = _barcode;
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        // Generate a large inventory of tools
        var tools = GenerateInv(100000);

        // Define the search criteria for tool types and their quantities
        var searchCriteria = new Dictionary<int, int> { { 1, 30 }, { 7, 15 }, { 10, 8 } };

        // Iterate over different thread counts to measure performance
        foreach (var threads in new[] { 2, 3, 4, 6 })
        {
            SearchTools(tools, searchCriteria, threads);

            // Start a stopwatch to measure the execution time
            var stopwatch = Stopwatch.StartNew();
            var results = SearchTools(tools, searchCriteria, threads);
            stopwatch.Stop();

            // Display the time taken and number of items found
            Console.WriteLine($"---> [Time taken: {stopwatch.ElapsedMilliseconds} ms]");
            Console.WriteLine($"---> [Found {results.Count} items]\n");
        }
    }

    // Method to generate a list of Tool objects with random types and barcodes
    static List<Tool> GenerateInv(int size) 
    {
        var inventory = new List<Tool>();
        var random = new Random();
        for (int i = 0; i < size; i++) 
        {
            inventory.Add(new Tool(random.Next(1, 101), random.Next()));
        }
        return inventory; 
    }

    // Method to search for tools based on given criteria using parallel processing
    static ConcurrentBag<Tool> SearchTools(List<Tool> tools, Dictionary<int, int> searchCriteria, int threads)
    {
        // A thread-safe collection to store the results
        var result = new ConcurrentBag<Tool>(); 
        var tasks = new List<Task>();
        object lockObj = new(); 

        // Iterate through each search criterion
        foreach (var criterion in searchCriteria)
        {
            // Create multiple tasks based on the number of threads specified
            for (int i = 0; i < threads; i++) 
            {
                var task = Task.Run(() => 
                {
                    // Iterate through the tools
                    foreach (var tool in tools)
                    {
                        lock (lockObj) // Ensure thread safety when accessing shared data, like adding a tool. Only one thread can add a tool at a time.
                        {
                            // Check if the tool matches the criterion and if the required quantity is not met
                            if (tool.type == criterion.Key && result.Count(t => t.type == criterion.Key) < criterion.Value)
                            {
                                result.Add(tool); // Add the tool to the list
                            }
                        }
                    }
                });

                tasks.Add(task); // Add the task to the list of tasks
            }
        }

        Task.WaitAll(tasks.ToArray()); // Wait for all tasks from the array to complete

        // Check if the correct number of tools for each type was found
        foreach (var criterion in searchCriteria)
        {
            if (result.Count(t => t.type == criterion.Key) != criterion.Value)
            {
                Console.WriteLine($"Warning: Did not find the required number of items for type {criterion.Key}");
            }
        }

        return result;
    }
}