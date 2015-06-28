﻿using System.Threading.Tasks;
using Set.ViewModels;
using Xamarin.Forms;

namespace Set
{
	public partial class CalendarNotesPage : ContentPage
	{
        public CalendarNotesViewModel ViewModel { get; set; }

		public CalendarNotesPage()
		{
			InitializeComponent ();
		}

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            BindingContext = ViewModel;

			CommentEditor.Focus ();

			// following statement will prevent a compiler warning about async method lacking await
			await Task.FromResult(0);
        }
	}
}

