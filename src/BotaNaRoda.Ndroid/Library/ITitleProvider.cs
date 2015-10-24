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

namespace BotaNaRoda.Ndroid.Library
{
    /**
	 * A TitleProvider provides the title to display according to a view.
	 */
    public interface ITitleProvider
    {
        /**
	     * Returns the title of the view at position
	     * @param position
	     * @return
	     */
        String GetTitle(int position);
    }
}

