using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using CommunityToolkit.Maui;

namespace BouncyBall
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp() => MauiApp.CreateBuilder()
			.UseMauiApp<App>()
			.UseMauiCompatibility()
			.Build();
	}
}