using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cirrious.MvvmCross.ViewModels;

namespace BotaNaRoda.UI.Core.ViewModels
{
    public class ItemsViewModel : MvxViewModel
    {
        public List<ItemsViewModel> Items { get; set; }

        public override void Start()
        {
            Items = new List<ItemsViewModel>();
            base.Start();
        }

    }
}
