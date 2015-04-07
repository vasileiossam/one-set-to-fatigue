﻿using System;
using System.Collections.Generic;
using Set.ViewModels;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace Set.Views
{
	public partial class ErrorPage : ContentPage
	{
		public ErrorViewModel ViewModel { get; set; }

		public ErrorPage ()
		{
			InitializeComponent ();
		}

		protected async override void OnAppearing()
		{
			base.OnAppearing();
			BindingContext = ViewModel;

			// following statement will prevent a compiler warning about async method lacking await
			await Task.FromResult(0);
		}

	}
}

