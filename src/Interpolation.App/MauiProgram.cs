using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
#if ANDROID
using Android.Views.InputMethods;
#endif

namespace Interpolation.App;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if ANDROID
		// Force the IME action label to show as "Enter" on all Entry fields
		EntryHandler.Mapper.AppendToMapping("ForceEnterLabel", (handler, view) =>
		{
			var native = handler.PlatformView;
			if (native is not null)
			{
				native.ImeOptions |= ImeAction.Done;
				native.SetImeActionLabel("Enter", ImeAction.Done);
			}
		});
#endif

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
