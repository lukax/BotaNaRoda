using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cirrious.MvvmCross.ViewModels;

namespace BotaNaRoda.UI.Core.ViewModels
{
    public class ItemListViewModel : MvxViewModel
    {
        public List<ItemListViewModel> Items { get; set; }

        public override void Start()
        {
            Items = new List<ItemListViewModel>();
            base.Start();
        }

    }
}
