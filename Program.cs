using Simulator_Ria;
using Simulator_Ria.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

const string API_BASE_URL = "http://localhost:5195/";
const string END_POINT = "Customer/";
const int MAX_CONCURRENT_REQUESTS = 5;
const int TOTAL_REQUESTS = 10;

HttpClient httpClient = new HttpClient { BaseAddress = new Uri(API_BASE_URL) };

async Task SimulatePostRequest()
{
    try
    {
        List<Customer> customers = new Data().GenerateRandomCustomers();

        CreateCustomersCommand command = new CreateCustomersCommand(customers);

        var json = JsonSerializer.Serialize(command);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        Console.WriteLine($"Request: POST {command.Customers.Count} customers\n");
        var response = await httpClient.PostAsync($"{END_POINT}CreateCustomers", content);

        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync();
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
            string error = await response.Content.ReadAsStringAsync();
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
    List<Task> tasks = new List<Task>();

    SemaphoreSlim semaphore = new SemaphoreSlim(MAX_CONCURRENT_REQUESTS);

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
    Console.WriteLine("All requests completed.\n Press any key to exit...");
    Console.ReadKey();
}

await Main();