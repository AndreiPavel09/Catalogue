// FrontEnd/Client/Program.cs

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FrontEnd.Services; // Make sure this namespace is correct for AdminApiClient
using System.Net.Http;    // Add this using statement

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// ****** START: Add these lines ******
// Configure HttpClient to point to your backend API
// IMPORTANT: Replace with your actual backend URL
builder.Services.AddScoped(sp => new HttpClient
{
    // This is the BASE URL of your ASP.NET Core Backend API project
    BaseAddress = new Uri("http://localhost:5039") // <--- CHANGE THIS TO YOUR BACKEND URL (use https if applicable)
});

// Register your API client service for dependency injection in Wasm components
builder.Services.AddScoped<AdminApiClient>();
// ****** END: Add these lines ******


await builder.Build().RunAsync();