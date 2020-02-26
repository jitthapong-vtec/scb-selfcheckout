using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SelfCheckout.Services.Localize
{
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        readonly CultureInfo ci = null;
        public const string ResourceId = "SelfCheckout.Resources.AppResources";
        
        static readonly Lazy<ResourceManager> ResMgr = new Lazy<ResourceManager>(
            () => new ResourceManager(ResourceId, IntrospectionExtensions.GetTypeInfo(typeof(TranslateExtension)).Assembly));

        public TranslateExtension()
        {
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS 
                || Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
            {
                ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            }
        }

        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null)
                return "";

            var translation = ResMgr.Value.GetString(Text, ci);
            if (translation == null)
            {
                throw new ArgumentException(
                    $"Key {Text} was not found in resources {ResourceId} for culture {ci.Name}.");
            }
            return translation;
        }
    }
}
