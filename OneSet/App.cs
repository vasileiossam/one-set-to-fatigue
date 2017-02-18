﻿using System;
using System.Diagnostics;
using System.Globalization;
using OneSet.Localization;
using OneSet.Models;
using OneSet.Resx;
using OneSet.ViewModels;
using OneSet.Views;
using Xamarin.Forms;
using Plugin.Toasts;

namespace OneSet
{
	public class App : Application
	{
		public static Database Database = new Database ();
		public static DateTime CurrentDate {get; set;}
		public static Settings Settings { get; set; }
		public static string Version {get; set;}
		public static double ScreenWidth {get; set;}
		public static double ScreenHeight {get; set;}
		public static int? TotalTrophies {get; set;}

		public static int RestTimerSecondsLeft { get; set; }
			
		public App ()
		{
#if DEBUG
            // That's the only way to have the breakpoints working...
            Debugger.Break ();
#endif
            Settings = DependencyService.Get<ISettingsStorage>().Load();
			L10n.SetLocale ();

			var netLanguage = DependencyService.Get<ILocale>().GetCurrent();
		    AppResources.Culture = new CultureInfo (netLanguage);

		    var mainNav = new NavigationPage(new WorkoutListPage())
		    {
		        BarBackgroundColor = ColorPalette.Primary,
		        BarTextColor = ColorPalette.Icons
		    };

		    MainPage = mainNav;
		}

		protected override void OnStart ()
		{
			CurrentDate = DateTime.Today;
			RestTimerSecondsLeft = 0;
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
 
        public static async void ShowToast(ToastNotificationType type, string title, string message)
		{
		    var notificator = DependencyService.Get<IToastNotificator>();
            var options = new NotificationOptions()
            {
                Title = title,
                Description = message,
                IsClickable = true,
            };
            await notificator.Notify(options);
		}

	    public static void SaveSettings()
		{
			DependencyService.Get<ISettingsStorage> ().Save(Settings);
		}

		public static void ShowErrorPage(object sender, Exception ex)
		{
			var viewModel = new ErrorViewModel () { Sender = sender, Exception = ex };

			var mainPage = Current.MainPage;
			if (mainPage != null)
			{
				mainPage.Navigation.PushAsync (new ErrorPage (){ ViewModel = viewModel });
			} 
			else
			{
				var mainNav = new NavigationPage (new ErrorPage (){ ViewModel = viewModel });
				Current.MainPage = mainNav;
			}
		}
	}
}