using System.Linq;
using Xamarin.Forms;

namespace SelfCheckout.Effects
{
    public class RoundCornersEffect : RoutingEffect
    {
        protected RoundCornersEffect() : base("SelfCheckout.Effects.RoundCornersEffect")
        {
        }

        public static readonly BindableProperty CornerRadiusProperty =
            BindableProperty.CreateAttached(
                "CornerRadius",
                typeof(int),
                typeof(RoundCornersEffect),
                0,
                propertyChanged: OnCornerRadiusChanged);

        public static readonly BindableProperty RoundCornerProperty =
            BindableProperty.CreateAttached(
                "RoundCorner",
                typeof(RoundCorners),
                typeof(RoundCornersEffect),
                RoundCorners.None);

        public static int GetCornerRadius(BindableObject view) =>
            (int)view.GetValue(CornerRadiusProperty);

        public static void SetCornerRadius(BindableObject view, int value) =>
            view.SetValue(CornerRadiusProperty, value);

        public static RoundCorners GetRoundCorner(BindableObject view) =>
            (RoundCorners)view.GetValue(RoundCornerProperty);

        public static void SetRoundCorner(BindableObject view, RoundCorners value) =>
            view.SetValue(RoundCornerProperty, value);

        private static void OnCornerRadiusChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is View view))
                return;

            var cornerRadius = (int)newValue;
            var effect = view.Effects.OfType<RoundCornersEffect>().FirstOrDefault();

            if (cornerRadius > 0 && effect == null)
                view.Effects.Add(new RoundCornersEffect());

            if (cornerRadius == 0 && effect != null)
                view.Effects.Remove(effect);
        }
    }
}
