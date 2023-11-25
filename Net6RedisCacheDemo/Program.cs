using StackExchange.Redis;


internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Starting configuration!");

        var configuration = ConfigurationOptions.Parse("localhost:6379");

        var redisConnection = ConnectionMultiplexer.Connect(configuration);

        Console.WriteLine("Starting redis!");

        var redisCache = redisConnection.GetDatabase();

        Console.WriteLine("Redis started!");

        Console.WriteLine("Fetching data with caching:");

        var cachedData = GetDataWithCaching(redisCache);
        Console.WriteLine($"Result: {cachedData}");

        Console.WriteLine("Fetching data without caching:");
        var uncachedData = GetDataFromDatabase();

        Console.WriteLine($"Result: {uncachedData}");
        redisConnection.Close(); //It is important to close the connection

        static string GetDataFromDatabase()
        {
            // Simulate fetching data from the database

            Thread.Sleep(1000); // Simulating latency

            return "Data from database";
        }


        static string GetDataWithCaching(IDatabase redisCache)
        {
            string cachedData = redisCache!.StringGet("cachedData");
            if (string.IsNullOrEmpty(cachedData))
            {
                cachedData = GetDataFromDatabase();
                redisCache.StringSet("cachedData", cachedData, TimeSpan.FromMinutes(10));
            }
            return cachedData;
        }
    }
}