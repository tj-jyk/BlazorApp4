using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Globalization;
using Microsoft.JSInterop;

namespace BlazorApp4.Client
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			builder.RootComponents.Add<App>("app");

			builder.Services.AddHttpClient("BlazorApp4.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
					.AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

			// Supply HttpClient instances that include access tokens when making requests to the server project
			builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazorApp4.ServerAPI"));

			builder.Services.AddApiAuthorization();

			builder.Services.AddLocalization();

			var host = builder.Build();
			var jsInterop = host.Services.GetRequiredService<IJSRuntime>();
			var result = await jsInterop.InvokeAsync<string>("blazorCulture.get");
			if (result != null)
			{
				var culture = new CultureInfo(result);
				CultureInfo.DefaultThreadCurrentCulture = culture;
				CultureInfo.DefaultThreadCurrentUICulture = culture;
			}

			await host.RunAsync();
		}
	}
}
