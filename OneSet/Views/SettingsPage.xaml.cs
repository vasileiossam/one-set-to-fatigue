﻿using OneSet.Abstract;
using OneSet.Models;
using OneSet.ViewModels;
using Xamarin.Forms;

namespace OneSet.Views
{
	public partial class SettingsPage : SettingsPageXaml
    {
        private readonly IMessagingService _messagingService;

        public SettingsPage(IMessagingService messagingService)
        {
            InitializeComponent();

            _messagingService = messagingService;

            _messagingService.Subscribe<SettingsViewModel>(this, Messages.SettingsReloaded, sender =>
            {
                Refresh();
            });
        }

        ~SettingsPage()
        {
            _messagingService.Unsubscribe<SettingsViewModel>(this, Messages.SettingsReloaded);
        }

        protected override void OnAppearing()
		{
			base.OnAppearing();

            BindingContext = ViewModel;
            settingsList.ItemsSource = ViewModel.Settings;
            settingsList.SelectedItem = null;
        }

        public void Refresh()
        {
            settingsList.ItemsSource = null;
            settingsList.ItemsSource = ViewModel.Settings;
        }

        private void SettingsList_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // unselect row
            settingsList.SelectedItem = null;
        }
    }

    public class SettingsPageXaml : BasePage<SettingsViewModel>
    {
    }
}

