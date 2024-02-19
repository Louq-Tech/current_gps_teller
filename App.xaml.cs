using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Graphics;
using Microsoft.Maui;

#if ANDROID
using Android.Graphics;
using Android.Content.Res;
using Android.Graphics.Drawables;
#endif

namespace Current_GPS_Teller
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

            // Remove Entry control underline
            EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#elif WINDOWS
                var entry = (Microsoft.UI.Xaml.Controls.TextBox)handler.PlatformView;
                entry.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
#elif IOS
                var entry = (UIKit.UITextField)handler.PlatformView;
                entry.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
            });

            // Set Entry control cursor color to white
            EntryHandler.Mapper.AppendToMapping("CursorColor", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.TextCursorDrawable.SetTint(Android.Graphics.Color.White);
#elif IOS
                handler.PlatformView.TintColor = UIKit.UIColor.White;
#endif
            });
        }
    }
}
