﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Xamarin.Forms;
using System.Linq;
using OneSet.Abstract;
using OneSet.Models;
using OneSet.Resx;
using OneSet.Services;

namespace OneSet.ViewModels
{
	public class ExerciseDetailsViewModel : BaseViewModel
	{
        private readonly INavigationService _navigationService;
        private readonly IUnitsService _units;
        private readonly IExercisesRepository _exercisesRepository;
        private readonly IRoutineDaysRepository _routineDaysRepository;

        public ExerciseDetailsViewModel(INavigationService navigationService, IUnitsService units, 
            IExercisesRepository exercisesRepository, IRoutineDaysRepository routineDaysRepository)
        {
            _navigationService = navigationService;
            _units = units;
            _exercisesRepository = exercisesRepository;
            _routineDaysRepository = routineDaysRepository;
            _routineDays = new List<RoutineDay>();
        }

        // TODO replace this with MessagingCenter
        // https://forums.xamarin.com/discussion/22499/looking-to-pop-up-an-alert-like-displayalert-but-from-the-view-model-xamarin-forms-labs
        [IgnoreMap]
    	public Views.ExerciseDetailsPage Page { get; set; }

		#region Exercice model
		public int ExerciseId { get; set; }
		public string Name { get; set; }
		public string Notes { get; set; }
		public double PlateWeight {get; set; }
        public int? RepsIncrementId { get; set; }
        public int? MaxReps { get; set; }
        public int? MinReps { get; set; }
        public int? RestTimerId { get; set; }
        #endregion

        [IgnoreMap]
        public RepsIncrement RepsIncrement
        {
            get
            {
                var repsIncrement = App.Database.RepsIncrements.FirstOrDefault(x => x.RepsIncrementId == RepsIncrementId);
                return repsIncrement;
            }
        }
        
        public bool NotesVisible => !string.IsNullOrEmpty (Notes);

	    private List<RoutineDay> _routineDays;

		[IgnoreMap]
        public bool DoOnMon { get; set; }
		[IgnoreMap]
		public bool DoOnTue { get; set; }
		[IgnoreMap]
		public bool DoOnWed { get; set; }
		[IgnoreMap]
		public bool DoOnThu { get; set; }
		[IgnoreMap]
		public bool DoOnFri { get; set; }
		[IgnoreMap]
		public bool DoOnSat { get; set; }
		[IgnoreMap] 
		public bool DoOnSun { get; set; }

        public string TrainingDays 
        { 
            get
            {
				var list = new List<string>();
				var dayNames = AppResources.Culture.DateTimeFormat.AbbreviatedDayNames;

				if (DoOnMon) { list.Add(dayNames[1]); }
                if (DoOnTue) { list.Add(dayNames[2]); }
				if (DoOnWed) { list.Add(dayNames[3]); }
                if (DoOnThu) { list.Add(dayNames[4]); }
                if (DoOnFri) { list.Add(dayNames[5]); }
                if (DoOnSat) { list.Add(dayNames[6]); }
                if (DoOnSun) { list.Add(dayNames[0]); }
                
				var s = string.Join(", ", list);
                
                // replace last , with 'and'
                if (s == string.Empty) return s;
                var place = s.LastIndexOf(",", StringComparison.Ordinal);
                if (place >= 0)
                {
                    s = s.Remove(place, 1).Insert(place, " " + AppResources.And);
                }

                return s;
            }
        }
		public bool TrainingDaysVisible => !string.IsNullOrEmpty (TrainingDays);

	    protected ICommand _deleteCommand;
		public ICommand DeleteCommand { 
			get { return _deleteCommand ?? (_deleteCommand = new Command(() => OnDelete())); }
		}

        public override async Task OnLoad(object parameter)
		{
			await LoadRoutine ();
			PlateWeight = _units.GetWeight(App.Settings.IsMetric, PlateWeight);
		}

		private async Task LoadRoutine()
		{
			_routineDays = await _routineDaysRepository.GetRoutine(ExerciseId);

			DoOnMon = _routineDays.Exists (x => (x.DayOfWeek == 1) && (x.IsActive == 1));
			DoOnTue = _routineDays.Exists (x => (x.DayOfWeek == 2) && (x.IsActive == 1));
			DoOnWed = _routineDays.Exists (x => (x.DayOfWeek == 3) && (x.IsActive == 1));
			DoOnThu = _routineDays.Exists (x => (x.DayOfWeek == 4) && (x.IsActive == 1));
			DoOnFri = _routineDays.Exists (x => (x.DayOfWeek == 5) && (x.IsActive == 1));
			DoOnSat = _routineDays.Exists (x => (x.DayOfWeek == 6) && (x.IsActive == 1));
			DoOnSun = _routineDays.Exists (x => (x.DayOfWeek == 0) && (x.IsActive == 1));
		}

		private async Task SaveRoutine()
		{
			// insert
			if (_routineDays.Count == 0)
			{
				await CreateRoutineDay(ExerciseId, 1, DoOnMon);
				await CreateRoutineDay(ExerciseId, 2, DoOnTue);
				await CreateRoutineDay(ExerciseId, 3, DoOnWed);
				await CreateRoutineDay(ExerciseId, 4, DoOnThu);
				await CreateRoutineDay(ExerciseId, 5, DoOnFri);
				await CreateRoutineDay(ExerciseId, 6, DoOnSat);
				await CreateRoutineDay(ExerciseId, 0, DoOnSun);
			} 
			// update
			else
			{
				foreach (var routineDay in _routineDays)
				{
					routineDay.IsActive = GetActive (routineDay.DayOfWeek);
					await _routineDaysRepository.SaveAsync(routineDay);
				}
			}
		}

		public async Task CreateRoutineDay(int exerciseId, int dayOfWeek, bool isActive)
		{
			var routineDay = new RoutineDay
			{
				ExerciseId = exerciseId,
				DayOfWeek = dayOfWeek,
				RowNumber = 1, 
				IsActive = isActive ? 1 : 0 
			};

			await _routineDaysRepository.SaveAsync(routineDay);
		}

		public int GetActive(int dayOfWeek)
		{
			var day = false;

			switch (dayOfWeek)
			{
				case 1: 
					day = DoOnMon;
					break;
				case 2: 
					day = DoOnTue;
					break;
				case 3: 
					day = DoOnWed;
					break;
				case 4: 
					day = DoOnThu;
					break;
				case 5: 
					day = DoOnFri;
					break;
				case 6: 
					day = DoOnSat;
					break;
				case 0: 
					day = DoOnSun;
					break;
			}

			return day ? 1 : 0;
		}

		private bool Validate ()
		{
			if (string.IsNullOrWhiteSpace (Name))
			{
				App.ShowWarning(AppResources.ExerciseNameIsRequired);
				return false;
			}

			if (PlateWeight < 0 || PlateWeight > 999)
			{
				App.ShowWarning(AppResources.ExerciseInvalidPlateWeight);
				PlateWeight = 0;
				return false;
			}

			return true;
		}

        public override async Task OnSave () 
		{
            if (Validate())
            {
                var exercise = Mapper.Map<Exercise>(this);

                // imperial to metric - always save in metric
                if (!App.Settings.IsMetric)
                {
                    exercise.PlateWeight = PlateWeight / _units.ImperialMetricFactor;
                }

                ExerciseId = await _exercisesRepository.SaveAsync(exercise);
                await SaveRoutine();

                await App.ShowSuccess(AppResources.ExerciseSaved);
                await _navigationService.PopAsync();
            }
        }

		protected virtual async Task OnDelete () 
		{
			try
			{
				var answer = await Page.DisplayAlert (AppResources.ExerciseDeleteQuestionTitle, AppResources.ExerciseDeleteQuestion, AppResources.Yes, AppResources.No);

				if (answer)
				{
					await _exercisesRepository.DeleteAsync(ExerciseId);
					await _navigationService.PopAsync();
				}
			}
			catch(Exception ex)
			{
				App.ShowErrorPage (this, ex);
			}
		}
	}
}
