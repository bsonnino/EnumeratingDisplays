using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EnumeratingDisplays
{
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
            var displayList = new DisplayList();
            _displays = displayList.Displays;
            InitializeCanvasWithDisplays();
        }

        private void InitializeCanvasWithDisplays()
        {
            var minX = 0;
            var minY = 0;
            var maxX = 0;
            var maxY = 0;
            foreach (var display in _displays)
            {
                if (minX > display.WorkingArea.X)
                    minX = display.WorkingArea.X;
                if (minY > display.WorkingArea.Y)
                    minY = display.WorkingArea.Y;
                if (maxX < display.WorkingArea.X + display.WorkingArea.Width)
                    maxX = display.WorkingArea.X + display.WorkingArea.Width;
                if (maxY < display.WorkingArea.Y + display.WorkingArea.Height)
                    maxY = display.WorkingArea.Y + display.WorkingArea.Height;
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
