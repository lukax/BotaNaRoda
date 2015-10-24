using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace BotaNaRoda.Ndroid.Util
{
    public static class UiUtils
    {
        public static ProgressDialog CreateProgressDialog(Context context)
        {
            var progressDialog = new ProgressDialog(context);
            try
            {
                progressDialog.Show();
            }
            catch (WindowManagerBadTokenException)
            {

            }

            progressDialog.SetCancelable(false);
            progressDialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            progressDialog.SetContentView(Resource.Layout.UtilProgressDialog);

            return progressDialog;
        }
    }
}