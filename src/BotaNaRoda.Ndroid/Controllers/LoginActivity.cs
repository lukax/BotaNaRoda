using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
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
        public const string PendingActionBundleKey = "br.com.botanaroda.ndroid:PendingAction";
		private UserRepository _userRepository;
	    private ICallbackManager _callbackManager;
        private PendingAction _pendingAction = PendingAction.None;
	    private CustomAccessTokenTracker _accessTokenTracker;
	    private ProgressBar _progressBar;
	    private LinearLayout _loginButtonsLinearLayout;

	    public enum PendingAction
        {
            None,
            PostItem
        }

        protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

            _userRepository = new UserRepository();

            FacebookSdk.SdkInitialize(ApplicationContext);

            _callbackManager = CallbackManagerFactory.Create();

            var loginCallback = new FacebookCallback<LoginResult>
            {
                HandleSuccess = loginResult =>
                {
                    //UpdateUser(loginResult.AccessToken);
                    //HandlePendingAction();
                    //UpdateUI();
                },
                HandleCancel = () => 
                {
                    if (_pendingAction != PendingAction.None)
                    {
                        ShowAlert(
                            GetString(Resource.String.cancelled),
                            GetString(Resource.String.permission_not_granted));
                        _pendingAction = PendingAction.None;
                    }
                    UpdateUI();
                },
                HandleError = loginError => 
                {
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

            //---------------------------
            if (Intent != null)
            {
                var name = Intent.GetStringExtra(PendingActionBundleKey);
                if(name != null)
                    _pendingAction = (PendingAction)Enum.Parse(typeof(PendingAction), name);
            }
            if (bundle != null)
            {
                var name = bundle.GetString(PendingActionBundleKey);
                if (name != null)
                    _pendingAction = (PendingAction)Enum.Parse(typeof(PendingAction), name);
            }

            SetContentView(Resource.Layout.Login);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            _progressBar = FindViewById<ProgressBar>(Resource.Id.loginProgressBar);
            _loginButtonsLinearLayout = FindViewById<LinearLayout>(Resource.Id.loginButtonsLinearLayout);

            var loginButton = FindViewById<LoginButton>(Resource.Id.loginFacebookButton);
            loginButton.SetReadPermissions("public_profile", "email");
            float fbIconScale = 1.45F;
            var drawable = ContextCompat.GetDrawable(this, Resource.Drawable.com_facebook_button_icon);
            drawable.SetBounds(0, 0, (int)(drawable.IntrinsicWidth * fbIconScale),
                                     (int)(drawable.IntrinsicHeight * fbIconScale));
            loginButton.SetCompoundDrawables(drawable, null, null, null);
            loginButton.CompoundDrawablePadding = (Resources.GetDimensionPixelSize(Resource.Dimension.fb_margin_override_textpadding));
            loginButton.SetPadding(
                    Resources.GetDimensionPixelSize(Resource.Dimension.fb_margin_override_lr),
                    Resources.GetDimensionPixelSize(Resource.Dimension.fb_margin_override_top),
                    0,
                    Resources.GetDimensionPixelSize(Resource.Dimension.fb_margin_override_bottom));

            //_profileTracker = new CustomProfileTracker
            //{
            //    HandleCurrentProfileChanged = (oldProfile, currentProfile) => 
            //    {
            //        UpdateUI();
            //        HandlePendingAction();
            //    }
            //};

            _accessTokenTracker = new CustomAccessTokenTracker
            {
                HandleCurrentAccessTokenChanged = (oldAccessToken, currentAccessToken) =>
                {
                    UpdateUserAsync(currentAccessToken);
                }
            };
		}

	    private async void UpdateUserAsync(AccessToken accessToken)
	    {
	        if (accessToken == null)
	        {
	            _userRepository.DeleteExistingAccounts();
	            return;
	        }

            _progressBar.Visibility = ViewStates.Visible;
            _loginButtonsLinearLayout.Visibility = ViewStates.Gone;
	        try
	        {
	            var token = await IdSvrOAuth2Util.RequestTokenForFacebookGrantAsync(accessToken.Token);
	            var userInfo = await IdSvrOAuth2Util.GetUserInfoAsync(token.AccessToken);
	            _userRepository.Update(token, userInfo);

                UpdateUI();
                HandlePendingAction();
            }
	        catch
	        {
	            Toast.MakeText(this, "Não foi possível conectar-se :(", ToastLength.Long);
	        }

            _progressBar.Visibility = ViewStates.Gone;
            _loginButtonsLinearLayout.Visibility = ViewStates.Visible;
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

	    protected override void OnDestroy()
	    {
	        base.OnDestroy();
            _accessTokenTracker.StopTracking();
	    }

	    private void UpdateUI()
	    {
	        
	    }

        void ShowAlert(string title, string msg, string buttonText = null)
        {
            new AlertDialog.Builder(this)
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
                case PendingAction.PostItem:
                    PostItem();
                    break;
                case PendingAction.None:
                    Finish();
                    break;
            }
        }

	    private void PostItem()
	    {
            StartActivity(typeof(ItemCreateActivity));
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

    class CustomAccessTokenTracker : AccessTokenTracker
    {
        public delegate void CurrentAccessTokenChangedDelegate(AccessToken oldAccessToken, AccessToken curreAccessToken);

        public CurrentAccessTokenChangedDelegate HandleCurrentAccessTokenChanged { get; set; }

        protected override void OnCurrentAccessTokenChanged(AccessToken oldAccessToken, AccessToken currentAccessToken)
        {
            var p = HandleCurrentAccessTokenChanged;
            if (p != null)
                p(oldAccessToken, currentAccessToken);
        }
    }
}

