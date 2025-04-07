using Simulator_Ria.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

const string API_BASE_URL = "http://localhost:5195/";
const string END_POINT = "Customer/";
const int MAX_CONCURRENT_REQUESTS = 5;
const int TOTAL_REQUESTS = 10;
const int MIN_CUSTOMERS_PER_REQUEST = 2;
const int MAX_CUSTOMERS_PER_REQUEST = 5;
const int MIN_AGE = 10;
const int MAX_AGE = 90;


var firstNames = new[] { "Leia", "Sadie", "Jose", "Sara", "Frank", "Dewey", "Tomas", "Joel", "Lukas", "Carlos" };
var lastNames = new[] { "Liberty", "Ray", "Harrison", "Ronan", "Drew", "Powell", "Larsen", "Chan", "Anderson", "Lane" };

var httpClient = new HttpClient { BaseAddress = new Uri(API_BASE_URL) };
var random = new Random();
var idIncremental = 0;

List<Customer> GenerateRandomCustomers()
{
    var count = random.Next(MIN_CUSTOMERS_PER_REQUEST, MAX_CUSTOMERS_PER_REQUEST + 1);
    var customers = new List<Customer>(count);

    for (int i = 0; i < count; i++)
    {
        var firstName = firstNames[random.Next(firstNames.Length)];
        var lastName = lastNames[random.Next(lastNames.Length)];
        var age = random.Next(MIN_AGE, MAX_AGE + 1);
        idIncremental++;

        customers.Add(new Customer(lastName, firstName, age, idIncremental));
    }

    return customers;
}

async Task SimulatePostRequest()
{
    try
    {
        List<Customer> customers = GenerateRandomCustomers();

        CreateCustomersCommand command = new CreateCustomersCommand(customers);

        var json = JsonSerializer.Serialize(command);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        Console.WriteLine($"Request: POST {command.Customers.Count} customers\n");
        var response = await httpClient.PostAsync($"{END_POINT}CreateCustomers", content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error: {error}\n");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception in request: {ex.Message}\n");
    }
}

async Task SimulateGetRequest()
{
    try
    {
        Console.WriteLine($"Request: GET all customers\n");

        var response = await httpClient.GetAsync($"{END_POINT}Customers");

        if (response.IsSuccessStatusCode)
        {
            var customers = await response.Content.ReadFromJsonAsync<List<Customer>>();

            if (customers != null)
            {
                Console.WriteLine($"Retrieved {customers.Count} customers\n");

                foreach (var item in customers)
                {
                    Console.WriteLine($"Last Name: {item.LastName}\nFirst Name: {item.FirstName}\nAge: {item.Age}\nId: {item.Id}\n");
                }
            }
            else
            {
                Console.WriteLine("No customers retrieved\n");
            }
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error: {error}\n");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception in request: {ex.Message}\n");
    }
}

async Task Main()
{
    var tasks = new List<Task>();

    var semaphore = new SemaphoreSlim(MAX_CONCURRENT_REQUESTS);

    for (int i = 0; i < TOTAL_REQUESTS; i++)
    {
        await semaphore.WaitAsync(); 

        bool isPostRequest = i % 2 == 0;

        tasks.Add(Task.Run(async () =>
        {
            try
            {
                if (isPostRequest)
                    await SimulatePostRequest();
                else
                    await SimulateGetRequest();
            }
            finally
            {
                semaphore.Release();
            }
        }));

        await Task.Delay(TimeSpan.FromMilliseconds(100));
    }

    await Task.WhenAll(tasks);
    Console.WriteLine("Simulation complete. Press any key to exit...");
    Console.ReadKey();
}

await Main();