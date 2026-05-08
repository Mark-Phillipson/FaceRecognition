using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FaceRecognition.Client;
using FaceRecognition.Client.Services;
using Microsoft.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IIndexedDbService, IndexedDbService>();
builder.Services.AddScoped<IFaceService, FaceInteropService>();
builder.Services.AddScoped<IRecognitionService, RecognitionService>();

var host = builder.Build();
var indexedDb = host.Services.GetRequiredService<IIndexedDbService>();
await indexedDb.InitAsync();
await host.RunAsync();
