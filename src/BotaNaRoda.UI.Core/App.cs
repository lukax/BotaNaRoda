using System;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.CrossCore;
using BotaNaRoda.UI.Core.ViewModels;

namespace BotaNaRoda.UI.Core
{
	/// <summary>
	/// this class is responsible for providing:
	/// registration of which interfaces and implementations the app uses
	/// registration of which ViewModel the App will show when it starts
	/// control of how ViewModels are located - although most applications normally just use the default implementation of this supplied by the base MvxApplication class.
	/// </summary>
	public class App: MvxApplication
	{
		public App ()
		{
			Mvx.RegisterSingleton<IMvxAppStart> (new MvxAppStart<ItemsViewModel> ());
		}
	}
}

