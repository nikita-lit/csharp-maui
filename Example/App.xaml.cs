using Microsoft.Extensions.DependencyInjection;

namespace Example
{
    public partial class App : Application
    {
        public static IServiceProvider? Services { get; set; }

        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());
            window.Width = 400;
            window.Height = 750;
            return window;
        }
    }
}
