using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Microsoft.Maui.Controls;

namespace Example
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        public override bool DispatchKeyEvent(KeyEvent e)
        {
            var page = Shell.Current?.CurrentPage as SnakeGame.SnakeGamePage;
            if (e.Action == KeyEventActions.Down && page != null)
            {
                switch (e.KeyCode)
                {
                    case Keycode.DpadUp:
                        page.SetDirection(SnakeGame.Models.Snake.Up);
                        return true;
                    case Keycode.DpadDown:
                        page.SetDirection(SnakeGame.Models.Snake.Down);
                        return true;
                    case Keycode.DpadLeft:
                        page.SetDirection(SnakeGame.Models.Snake.Left);
                        return true;
                    case Keycode.DpadRight:
                        page.SetDirection(SnakeGame.Models.Snake.Right);
                        return true;
                }
            }

            return base.DispatchKeyEvent(e);
        }
    }
}
