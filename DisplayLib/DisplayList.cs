using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;

namespace EnumeratingDisplays
{
    public record Rect(int X, int Y, int Width, int Height);
    public record Display(string DeviceName, Rect Bounds, Rect WorkingArea, double ScalingFactor);


    public class DisplayList
    {
        private List<Display> _displays = new List<Display>();

        public DisplayList()
        {
            QueryDisplayDevices();
        }

        public List<Display> Displays => _displays;

        private unsafe void QueryDisplayDevices()
        {
            PInvoke.EnumDisplayMonitors(null, null, enumProc, IntPtr.Zero);
        }

        private unsafe BOOL enumProc(HMONITOR monitor, HDC hdc, RECT* clipRect, LPARAM data)
        {
            var mi = new MONITORINFOEXW();
            mi.monitorInfo.cbSize = (uint)Marshal.SizeOf(typeof(MONITORINFOEXW));

            if (PInvoke.GetMonitorInfo(monitor, (MONITORINFO*)&mi))
            {
                var dm = new DEVMODEW
                {
                    dmSize = (ushort)Marshal.SizeOf(typeof(DEVMODEW))
                };
                PInvoke.EnumDisplaySettings(mi.szDevice.ToString(), ENUM_DISPLAY_SETTINGS_MODE.ENUM_CURRENT_SETTINGS, ref dm);

                var scalingFactor = Math.Round((double)dm.dmPelsWidth / mi.monitorInfo.rcMonitor.Width, 2);
                var display = new Display(mi.szDevice.ToString(),
                    new Rect(mi.monitorInfo.rcMonitor.left, mi.monitorInfo.rcMonitor.top, 
                        (int)(mi.monitorInfo.rcMonitor.Width * scalingFactor), (int)(mi.monitorInfo.rcMonitor.Height * scalingFactor)),
                    new Rect(mi.monitorInfo.rcWork.left, mi.monitorInfo.rcWork.top, 
                        (int)(mi.monitorInfo.rcWork.Width * scalingFactor), (int)(mi.monitorInfo.rcWork.Height * scalingFactor)),
                    scalingFactor);
                _displays.Add(display);
            }
            return true;
        }
    }

}
