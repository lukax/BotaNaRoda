using Android.OS;
using Android.Support.V4.App;
using Android.Views;

namespace BotaNaRoda.Ndroid
{
    public class ChatFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.Chat, container, false);

            return view;
        }
    }
}