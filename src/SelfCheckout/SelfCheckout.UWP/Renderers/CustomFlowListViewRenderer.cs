using SelfCheckout.Controls;
using SelfCheckout.UWP.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly:ExportRenderer(typeof(CustomFlowListView), typeof(CustomFlowListViewRenderer))]
namespace SelfCheckout.UWP.Renderers
{
    public class CustomFlowListViewRenderer : ListViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            if (List != null)
            {
                List.SelectionMode = Windows.UI.Xaml.Controls.ListViewSelectionMode.None;
                List.IsItemClickEnabled = false;
            }
        }
    }
}
