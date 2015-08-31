using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Cirrious.MvvmCross.Droid.Platform;
using Cirrious.MvvmCross.ViewModels;
using BotaNaRoda.UI.Core;

namespace BotaNaRoda.UI.Ndroid
{
	/// <summary>
	/// the Inversion of Control (IoC) system
	/// the MvvmCross data-binding
	/// your App and its collection of ViewModels
	/// your UI project and its collection of Views
	/// </summary>
    public class Setup : MvxAndroidSetup
    {
        public Setup(Context applicationContext) : base(applicationContext)
        {
        }

        protected override IMvxApplication CreateApp()
		{
			return new App ();
        }
    }
}