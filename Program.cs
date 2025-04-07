using Simulator_Ria.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

const string API_BASE_URL = "http://localhost:5195/";
const string END_POINT = "Customer/";
const int concurrentRequests = 5;
const int minCustomersPerRequest = 2;
const int maxCustomersPerRequest = 5;
const int minAge = 10;
const int maxAge = 90;

var firstNames = new[] { "Leia", "Sadie", "Jose", "Sara", "Frank", "Dewey", "Tomas", "Joel", "Lukas", "Carlos" };
var lastNames = new[] { "Liberty", "Ray", "Harrison", "Ronan", "Drew", "Powell", "Larsen", "Chan", "Anderson", "Lane" };

var httpClient = new HttpClient { BaseAddress = new Uri(API_BASE_URL) };
var random = new Random();
var idIncremental = 0; 

List<Customer> GenerateRandomCustomers()
{
    var count = random.Next(minCustomersPerRequest, maxCustomersPerRequest + 1);
    var customers = new List<Customer>(count);

    for (int i = 0; i < count; i++)
    {
        var firstName = firstNames[random.Next(firstNames.Length)];
        var lastName = lastNames[random.Next(lastNames.Length)];
        var age = random.Next(minAge, maxAge + 1);
        //var id = Interlocked.Increment(ref idCounter); 
        idIncremental++;

        customers.Add(new Customer(lastName,firstName,age, idIncremental));
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

        Console.WriteLine($"Request: POST {command.Customers.Count} customers");
        var response = await httpClient.PostAsync($"{END_POINT}CreateCustomers", content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error: {error}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception in request: {ex.Message}");
    }
}
async Task SimulateGetRequest()
{
    try
    {
        Console.WriteLine($"Request: GET all customers");

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
                Console.WriteLine("No customers retrieved");
            }
        }
        else {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error: {error}");
        }

     
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception in request: {ex.Message}");
    }
}
await SimulatePostRequest();

//// Run parallel simulations
//var tasks = new List<Task>();
//for (int i = 0; i < concurrentRequests; i++)
//{
//    // Alternate between POST and GET requests
//    if (i % 2 == 0)
//    {
//        tasks.Add(SimulatePostRequest());
//    }
//    else
//    {
//        tasks.Add(SimulateGetRequest());
//    }
//}

//await Task.WhenAll(tasks);

Console.WriteLine("Simulation complete. Press any key to exit...");
Console.ReadKey();




//using System.Net.Http.Json;

//var firstNames = new[] { "Leia", "Sadie", "Jose", "Sara", "Frank", "Dewey", "Tomas", "Joel", "Lukas", "Carlos" };
//var lastNames = new[] { "Liberty", "Ray", "Harrison", "Ronan", "Drew", "Powell", "Larsen", "Chan", "Anderson", "Lane" };

//var http = new HttpClient();
//int idCounter = 1;

//string RandomName(string[] names) =>
//    names[new Random().Next(names.Length)];

//List<Customer> GenerateCustomers()
//{
//    var rand = new Random();
//    int count = rand.Next(2, 5);
//    var list = new List<Customer>();

//    for (int i = 0; i < count; i++)
//    {
//        list.Add(new Customer
//        {
//            Id = idCounter++,
//            FirstName = RandomName(firstNames),
//            LastName = RandomName(lastNames),
//            Age = rand.Next(10, 91)
//        });
//    }

//    return list;
//}

//async Task Simulate()
//{
//    var post = http.PostAsJsonAsync("http://localhost:5000/customers", GenerateCustomers());
//    var get = http.GetAsync("http://localhost:5000/customers");

//    await Task.WhenAll(post, get);

//    var postRes = await post;
//    Console.WriteLine("POST: " + postRes.StatusCode);

//    var getRes = await get;
//    Console.WriteLine("GET: " + getRes.StatusCode);
//}

//List<Task> tasks = new();
//for (int i = 0; i < 5; i++)
//    tasks.Add(Simulate());

//await Task.WhenAll(tasks);

//public class Customer
//{
//    public string FirstName { get; set; } = default!;
//    public string LastName { get; set; } = default!;
//    public int Age { get; set; }
//    public int Id { get; set; }
//}
