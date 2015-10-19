using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using BotaNaRoda.Ndroid.Auth;
using BotaNaRoda.Ndroid.Data;
using Xamarin.Auth;
using Xamarin.Facebook;
using Xamarin.Facebook.AppEvents;
using Xamarin.Facebook.Login;
using Xamarin.Facebook.Login.Widget;
using AlertDialog = Android.App.AlertDialog;

namespace BotaNaRoda.Ndroid.Controllers
{
	[Activity (Label = "Entrar - Bota na Roda",
		ConfigurationChanges = (ConfigChanges.Orientation | ConfigChanges.ScreenSize),
        LaunchMode = Android.Content.PM.LaunchMode.SingleTask,
        ParentActivity = typeof(MainActivity),
        Theme = "@style/MainTheme")]
	public class LoginActivity : AppCompatActivity
	{
		private UserRepository _userRepository;
	    private ICallbackManager _callbackManager;
        const string PendingActionBundleKey = "br.com.botanaroda.ndroid:PendingAction";
        PendingAction _pendingAction = PendingAction.None;
	    private LoginButton _loginButton;

	    enum PendingAction
        {
            None,
            PostPhoto,
            PostStatusUpdate
        }

        protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

            if (bundle != null)
            {
                var name = bundle.GetString(PendingActionBundleKey);
                _pendingAction = (PendingAction)Enum.Parse(typeof(PendingAction), name);
            }

            _userRepository = new UserRepository();

            _loginButton = FindViewById<LoginButton>(Resource.Id.fbLoginButton);
            _loginButton.SetReadPermissions("public_profile", "email");

            FacebookSdk.SdkInitialize(ApplicationContext);

            _callbackManager = CallbackManagerFactory.Create();

            var loginCallback = new FacebookCallback<LoginResult>
            {
                HandleSuccess = loginResult => {
                    HandlePendingAction();
                    UpdateUI();
                },
                HandleCancel = () => {
                    if (_pendingAction != PendingAction.None)
                    {
                        ShowAlert(
                            GetString(Resource.String.cancelled),
                            GetString(Resource.String.permission_not_granted));
                        _pendingAction = PendingAction.None;
                    }
                    UpdateUI();
                },
                HandleError = loginError => {
                    if (_pendingAction != PendingAction.None
                        && loginError is FacebookAuthorizationException)
                    {
                        ShowAlert(
                            GetString(Resource.String.cancelled),
                            GetString(Resource.String.permission_not_granted));
                        _pendingAction = PendingAction.None;
                    }
                    UpdateUI();
                }
            };

            LoginManager.Instance.RegisterCallback(_callbackManager, loginCallback);

            //---
            SetContentView(Resource.Layout.Login);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutString(PendingActionBundleKey, _pendingAction.ToString());
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            _callbackManager.OnActivityResult(requestCode, (int)resultCode, data);
        }

        protected override void OnResume()
        {
            base.OnResume();
            AppEventsLogger.ActivateApp(this);
        }

        protected override void OnPause()
        {
            base.OnPause();
            AppEventsLogger.DeactivateApp(this);
        }

	    private void UpdateUI()
	    {
	        
	    }

        void ShowAlert(string title, string msg, string buttonText = null)
        {
            new AlertDialog.Builder(Parent)
                .SetTitle(title)
                .SetMessage(msg)
                .SetPositiveButton(buttonText, (s2, e2) => { })
                .Show();
        }

        private void HandlePendingAction()
        {
            PendingAction previouslyPendingAction = _pendingAction;
            // These actions may re-set pendingAction if they are still pending, but we assume they
            // will succeed.
            _pendingAction = PendingAction.None;

            switch (previouslyPendingAction)
            {
                case PendingAction.PostPhoto:
                    //PostPhoto();
                    break;
                case PendingAction.PostStatusUpdate:
                    //PostStatusUpdate();
                    break;
            }
        }

    }

    class FacebookCallback<TResult> : Java.Lang.Object, IFacebookCallback where TResult : Java.Lang.Object
    {
        public Action HandleCancel { get; set; }
        public Action<FacebookException> HandleError { get; set; }
        public Action<TResult> HandleSuccess { get; set; }

        public void OnCancel()
        {
            var c = HandleCancel;
            if (c != null)
                c();
        }

        public void OnError(FacebookException error)
        {
            var c = HandleError;
            if (c != null)
                c(error);
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            var c = HandleSuccess;
            if (c != null)
                c(result.JavaCast<TResult>());
        }
    }

    class CustomProfileTracker : ProfileTracker
    {
        public delegate void CurrentProfileChangedDelegate(Profile oldProfile, Profile currentProfile);

        public CurrentProfileChangedDelegate HandleCurrentProfileChanged { get; set; }

        protected override void OnCurrentProfileChanged(Profile oldProfile, Profile currentProfile)
        {
            var p = HandleCurrentProfileChanged;
            if (p != null)
                p(oldProfile, currentProfile);
        }
    }
}

