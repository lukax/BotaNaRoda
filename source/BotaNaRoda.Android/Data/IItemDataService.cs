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
using BotaNaRoda.Android.Entity;

namespace BotaNaRoda.Android.Data
{
    public interface IItemDataService
    {
		IReadOnlyList<Item> GetAllItems();
        void RefreshCache();
        Item GetItem(string id);
        void SaveItem(Item item);
        void DeleteItem(Item item);
    }
}