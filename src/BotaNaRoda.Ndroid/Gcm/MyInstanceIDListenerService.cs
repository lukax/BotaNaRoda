using Android.App;
using Android.Content;
using Android.Gms.Gcm.Iid;

namespace BotaNaRoda.Ndroid.Gcm
{
    [Service(Exported = false), IntentFilter(new[] { "com.google.android.gms.iid.InstanceID" })]
    public class MyInstanceIdListenerService : InstanceIDListenerService
    {
        public override void OnTokenRefresh()
        {
            var intent = new Intent(this, typeof(GcmRegistrationIntentService));
            StartService(intent);
        }
    }
}