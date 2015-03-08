﻿using System;
using System.Collections.Generic;
using Set.ViewModels;
using Set.Models;
using Xamarin.Forms;
using Set.Localization;
using Set.Resx;

namespace Set
{
	public partial class ExercisePage : ContentPage
	{
		private ExerciseViewModel _viewModel;
		public ExerciseViewModel ViewModel
		{
			get
			{
				if (_viewModel == null)
				{
					_viewModel =  new ExerciseViewModel(Navigation);
				}
				return _viewModel;
			}
			set
			{
				_viewModel = value;
			}
		}

		public ExercisePage ()
		{
			InitializeComponent ();
		}

        protected async override void OnAppearing()
        {
            base.OnAppearing();
			BindingContext = ViewModel;

			PlateWeightLabel.Text = string.Format (AppResources.PlateWeightLabel, L10n.GetWeightUnit());

			var dayNames = AppResources.Culture.DateTimeFormat.DayNames;
            MonLabel.Text = dayNames[1];
            TueLabel.Text = dayNames[2];
            WedLabel.Text = dayNames[3];
            ThuLabel.Text = dayNames[4];
            FriLabel.Text = dayNames[5];
            SatLabel.Text = dayNames[6];
            SunLabel.Text = dayNames[0];
        }

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
		}

		protected void OnPlateWeightStepperValueChanged(object sender, ValueChangedEventArgs args)
		{
			PlateWeightStepperLabel.Text = args.NewValue.ToString ();
		}


	}
}

