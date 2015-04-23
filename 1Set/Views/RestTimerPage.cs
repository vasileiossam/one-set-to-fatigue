﻿using System;
using System.Collections.Generic;
using Set.ViewModels;
using Set.Models;
using Xamarin.Forms;
using Set.Resx;
using System.Threading.Tasks;

namespace Set
{
	public partial class RestTimerPage : ContentPage
	{
		public RestTimerViewModel ViewModel { get; set; }

		public RestTimerPage()
		{
			InitializeComponent ();
		}

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            BindingContext = ViewModel;
			ViewModel.ProgressBar = ProgressBar;
			await ViewModel.Load ();

			// following statement will prevent a compiler warning about async method lacking await
			await Task.FromResult(0);
        }

		protected override void OnDisappearing()
		{
			if (ViewModel != null)
			{
				ViewModel.StopTimers ();
			}
			base.OnDisappearing();
		}
	}
}
