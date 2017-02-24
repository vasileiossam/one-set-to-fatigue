﻿using System;
using System.Threading.Tasks;
using System.Windows.Input;
using OneSet.Abstract;
using OneSet.Resx;
using Xamarin.Forms;

namespace OneSet.ViewModels
{
    public enum RestTimerStates
    {
        Paused,
        Running,
        Editing
    }

    public class RestTimerViewModel : BaseViewModel
	{
		public ProgressBar ProgressBar { get; set; }
		protected double _progressStep;

		protected RestTimerStates _state;
		public RestTimerStates State
		{
			get
			{
				return _state;
			}
			set
			{
			    if (_state == value) return;
			    _state = value;
			    OnPropertyChanged ("State");
			}
		}

		protected int _totalSeconds;
		public int TotalSeconds
		{
			get
			{
				return _totalSeconds;
			}
			set
			{
			    if (_totalSeconds == value) return;
			    _totalSeconds = value;
			    Save ();
			    OnPropertyChanged ("TotalSeconds");
			}
		}

		protected int _secondsLeft;
		public int SecondsLeft
		{
			get
			{
				return _secondsLeft;
			}
			set
			{
			    if (_secondsLeft == value) return;
			    _secondsLeft = value;
			    OnPropertyChanged ("SecondsLeft");
			}
		}

		protected bool? _playSounds;
		public bool? PlaySounds
		{
			get
			{
				return _playSounds;
			}
			set
			{
			    if (_playSounds == value) return;
			    _playSounds = value;
			    OnPropertyChanged("PlaySounds");

			    _playSoundsImage = _playSounds == true ? "sound" : "nosound";
			    OnPropertyChanged("PlaySoundsImage");
			}
		}

		protected string _playSoundsImage;
		public string PlaySoundsImage
		{
			get
			{
				return _playSoundsImage;
			}
			set
			{
			    if (_playSoundsImage == value) return;
			    _playSoundsImage = value;
			    OnPropertyChanged("PlaySoundsImage");
			}
		}

		protected bool _autoStart;
		public bool AutoStart
		{
			get
			{
				return _autoStart;
			}
			set
			{
			    if (_autoStart == value) return;
			    _autoStart = value;
			    Save ();
			    OnPropertyChanged("AutoStart");
			}
		}

		protected bool _canSave;

		public string MotivationalQuoteImageFile 
		{
			get
			{
			    switch (App.Settings.ImagePackId)
			    {
			        case 1:
			        {
			            var r = new Random ();
			            int i = r.Next (1, 25);
			            return $"QFitness_{i}.png";
			        }
			        case 2:
			        {
			            var r = new Random ();
			            int i = r.Next (1, 20);
			            return $"QInspirational_{i}.png";
			        }
			    }
			    return string.Empty;
			}
		}

		public bool MotivationalQuoteImageVisible => App.Settings.CanShowImagePackInRestTimer;

	    #region commands
		protected ICommand _startCommand;
		public ICommand StartCommand => _startCommand;

	    protected ICommand _pauseCommand;
		public ICommand PauseCommand => _pauseCommand;

	    protected ICommand _resetCommand;
		public ICommand ResetCommand => _resetCommand;

	    protected ICommand _playSoundsCommand;
		public ICommand PlaySoundsCommand => _playSoundsCommand;

	    protected ICommand _editingModeCommand;
		public ICommand EditingModeCommand => _editingModeCommand;

	    #endregion

		public RestTimerViewModel()
			: base()
		{
			Title = AppResources.RestTimerTitle;
		
			_startCommand = new Command (async() => { await OnStartCommand(); });
			_pauseCommand = new Command (async() => { await OnPauseCommand(); });
			_resetCommand = new Command (async() => { await OnResetCommand(); });

			_playSoundsCommand  = new Command (() => { 
				PlaySounds = !PlaySounds; 
				Save();
			});

			_editingModeCommand  = new Command (() => { 
				State = RestTimerStates.Editing;
			});
		}

        public override async Task OnLoad(object parameter = null)
		{
			_canSave = false;
			AutoStart = App.Settings.RestTimerAutoStart;
			PlaySounds = App.Settings.RestTimerPlaySounds;
			TotalSeconds = App.Settings.RestTimerTotalSeconds;

			// rest timer already running
			if (App.RestTimerSecondsLeft > 0)
			{
				var seconds = App.RestTimerSecondsLeft; 
				await OnResetCommand ();
				SecondsLeft = seconds;
				await OnStartCommand ();
			} else
			{
				await OnResetCommand ();
			}

			_canSave = true;
		}

		protected void Save()
		{
			if (!_canSave)
				return;
			App.Settings.RestTimerAutoStart = AutoStart;
			App.Settings.RestTimerPlaySounds = PlaySounds ?? false;
			App.Settings.RestTimerTotalSeconds = TotalSeconds;
			App.SaveSettings ();
		}

		protected bool OnTimer()
		{
			if (State != RestTimerStates.Running)
				return false;
			
			if ((SecondsLeft - 1) <= 0)
			{
				OnResetCommand ();

				if (PlaySounds == true)
				{
					var soundService = DependencyService.Get<ISoundService> ();
					soundService.Play ("Bleep");
				}

				App.RestTimerSecondsLeft = 0;
				return false;
			}

			SecondsLeft = SecondsLeft - 1;

			var progress = ProgressBar.Progress + _progressStep;
			ProgressBar.Progress = progress >= 1 ? 1 : progress;

			App.RestTimerSecondsLeft = SecondsLeft;
			return State == RestTimerStates.Running;	
		}

		public async Task OnStartCommand()
		{	
			if (State == RestTimerStates.Editing)
			{
				SecondsLeft = TotalSeconds;
				ProgressBar.Progress = 0;
				_progressStep = GetProgressStep ();
			}

			State = RestTimerStates.Running;
			Device.StartTimer(TimeSpan.FromSeconds(1), () => {	return OnTimer(); });

			// following statement will prevent a compiler warning about async method lacking await
			await Task.FromResult(0);
		}

		private async Task OnPauseCommand()
		{
			App.RestTimerSecondsLeft = 0;
			State = RestTimerStates.Paused;

			// following statement will prevent a compiler warning about async method lacking await
			await Task.FromResult(0);
		}

		private async Task OnResetCommand()
		{
			App.RestTimerSecondsLeft = 0;
			State = RestTimerStates.Paused;
			SecondsLeft = TotalSeconds;
			ProgressBar.Progress = 0;
			_progressStep = GetProgressStep ();

			// following statement will prevent a compiler warning about async method lacking await
			await Task.FromResult(0);
		}

		protected double GetProgressStep()
		{
			if (TotalSeconds == 0)
			{
				return 0;
			} else
			{
				return 1.0 / TotalSeconds;
			}			
		}

		public void StopTimer()
		{
			State = RestTimerStates.Paused;
		}

        public override Task OnSave()
	    {
	        throw new NotImplementedException();
	    }
	}
}

