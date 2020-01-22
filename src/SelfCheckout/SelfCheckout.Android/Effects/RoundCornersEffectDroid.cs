using Android.Graphics;
using Android.Views;
using SelfCheckout.Droid.Effects;
using SelfCheckout.Effects;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("SelfCheckout.Effects")]
[assembly: ExportEffect(typeof(RoundCornersEffectDroid), nameof(RoundCornersEffect))]
namespace SelfCheckout.Droid.Effects
{
    public class RoundCornersEffectDroid : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                PrepareContainer();
                SetCornerRadius();
            }
            catch { }
        }

        protected override void OnDetached()
        {
            try
            {
                Container.OutlineProvider = ViewOutlineProvider.Background;
            }
            catch { }
        }

        private void PrepareContainer()
        {
            Container.ClipToOutline = true;
        }

        private void SetCornerRadius()
        {
            var cornerRadius = RoundCornersEffect.GetCornerRadius(Element) * GetDensity();
            var roundCorner = RoundCornersEffect.GetRoundCorner(Element);

            Container.OutlineProvider = new RoundedOutlineProvider(roundCorner, cornerRadius);
        }

        private static float GetDensity() => (float)DeviceDisplay.MainDisplayInfo.Density;

        private class RoundedOutlineProvider : ViewOutlineProvider
        {
            private readonly float _radius;
            private readonly RoundCorners _roundCorner;

            public RoundedOutlineProvider(RoundCorners roundCorner, float radius)
            {
                _roundCorner = roundCorner;
                _radius = radius;
            }

            public override void GetOutline(Android.Views.View view, Outline outline)
            {
                var left = 0;
                var top = 0;
                var right = view.Width;
                var bottom = view.Height;

                switch (_roundCorner)
                {
                    case RoundCorners.Top:
                        bottom = (int)(view.Height + _radius);
                        break;
                    case RoundCorners.Bottom:
                        top = (int)_radius * -1;
                        break;
                    case RoundCorners.TopLeft:
                        right = (int)(view.Width + _radius);
                        bottom = (int)(view.Height + _radius);
                        break;
                    case RoundCorners.TopRight:
                        left = (int)_radius * -1;
                        bottom = (int)(view.Height + _radius);
                        break;
                    case RoundCorners.BottomLeft:
                        top = (int)_radius * -1;
                        right = (int)(view.Width + _radius);
                        break;
                    case RoundCorners.BottomRight:
                        left = (int)_radius * -1;
                        top = (int)_radius * -1;
                        break;
                }
                outline?.SetRoundRect(left, top, right, bottom, _radius);
            }
        }
    }
}