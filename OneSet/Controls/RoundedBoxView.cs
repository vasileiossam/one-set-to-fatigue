﻿using Xamarin.Forms;
using static Xamarin.Forms.BindableProperty;

namespace OneSet.Controls
{
	/// <summary>
	/// http://forums.xamarin.com/discussion/17792/video-on-making-custom-renderers
	/// </summary>
	public class RoundedBoxView : BoxView
	{
			public static readonly BindableProperty CornerRadiusProperty =
				Create<RoundedBoxView, double>(p => p.CornerRadius, 0);

			public double CornerRadius
			{
				get { return (double)base.GetValue(CornerRadiusProperty);}
				set { base.SetValue(CornerRadiusProperty, value);}
			}

			public static readonly BindableProperty StrokeProperty = 
				Create<RoundedBoxView,Color>(
					p => p.Stroke, Color.Transparent);

			public Color Stroke {
				get { return (Color)GetValue(StrokeProperty); }
				set { SetValue(StrokeProperty, value); }
			}

			public static readonly BindableProperty StrokeThicknessProperty = 
				Create<RoundedBoxView,double>(
					p => p.StrokeThickness, default(double));

			public double StrokeThickness {
				get { return (double)GetValue(StrokeThicknessProperty); }
				set { SetValue(StrokeThicknessProperty, value); }
			}

			public static readonly BindableProperty HasShadowProperty = 
				Create<RoundedBoxView,bool>(
					p => p.HasShadow, default(bool));

			public bool HasShadow {
				get { return (bool)GetValue(HasShadowProperty); }
				set { SetValue(HasShadowProperty, value); }
			}
		}
	}
