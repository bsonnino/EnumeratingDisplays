using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EnumeratingDisplays
{
    internal record Rect(int X, int Y, int Width, int Height);
    internal record Display(string DeviceName, Rect Bounds, Rect WorkingArea, double ScalingFactor);

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeDisplayCanvas();
        }

        List<Display> _displays = new List<Display>();

        private void InitializeDisplayCanvas()
        {
            InitializeCanvasWithWinForms();
        }

        private void InitializeCanvasWithWinForms()
        {
            var minX = 0;
            var minY = 0;
            var maxX = 0;
            var maxY = 0;
            foreach (var screen in Screen.AllScreens)
            {
                if (minX > screen.WorkingArea.X)
                    minX = screen.WorkingArea.X;
                if (minY > screen.WorkingArea.Y)
                    minY = screen.WorkingArea.Y;
                if (maxX < screen.WorkingArea.X + screen.WorkingArea.Width)
                    maxX = screen.WorkingArea.X + screen.WorkingArea.Width;
                if (maxY < screen.WorkingArea.Y + screen.WorkingArea.Height)
                    maxY = screen.WorkingArea.Y + screen.WorkingArea.Height;
                
                _displays.Add(new Display(screen.DeviceName,
                    new Rect(screen.Bounds.Left,
                                 screen.Bounds.Top,
                                 screen.Bounds.Width,
                                 screen.Bounds.Height),
                        new Rect(screen.WorkingArea.Left,
                                 screen.WorkingArea.Top,
                                 screen.WorkingArea.Width,
                                 screen.WorkingArea.Height), 1));
            }
            DisplayCanvas.Width = maxX - minX;
            DisplayCanvas.Height = maxY - minY;
            DisplayCanvas.RenderTransform = new TranslateTransform(-minX, -minY);
            var background = new System.Windows.Shapes.Rectangle
            {
                Width = DisplayCanvas.Width,
                Height = DisplayCanvas.Height,
                Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(1, 242, 242, 242)),
            };
            Canvas.SetLeft(background, minX);
            Canvas.SetTop(background, minY);
            DisplayCanvas.Children.Add(background);
            var numDisplay = 0;
            foreach (var display in _displays)
            {
                numDisplay++;
                var border = new Border
                {
                    Width = display.WorkingArea.Width,
                    Height = display.WorkingArea.Height,
                    Background = System.Windows.Media.Brushes.DarkGray,
                    CornerRadius = new CornerRadius(30)
                };
                var text = new TextBlock
                {
                    Text = numDisplay.ToString(),
                    FontSize = 200,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                border.Child = text;
                Canvas.SetLeft(border, display.WorkingArea.X);
                Canvas.SetTop(border, display.WorkingArea.Y);
                DisplayCanvas.Children.Add(border);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var display = _displays[0];
            var window = new NewWindow
            {
                Top = display.Bounds.Y + (display.Bounds.Height - 200) / display.ScalingFactor / 2,
                Left = display.Bounds.X + (display.Bounds.Width - 200) / display.ScalingFactor / 2,
            };
            window.Show();
        }
    }
}
