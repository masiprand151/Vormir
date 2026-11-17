using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Vormir.Utils;

public static class WindowPositionHelper
{
  [StructLayout(LayoutKind.Sequential)]
  private struct RECT
  {
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
  }

  [StructLayout(LayoutKind.Sequential)]
  private struct WINDOWINFO
  {
    public uint cbSize;
    public RECT rcWindow;
    public RECT rcClient;
    public uint dwStyle;
    public uint dwExStyle;
    public uint dwWindowStatus;
    public uint cxWindowBorders;
    public uint cyWindowBorders;
    public ushort atomWindowType;
    public ushort wCreatorVersion;
  }

  [DllImport("user32.dll", SetLastError = true)]
  private static extern bool GetWindowInfo(
      IntPtr hwnd,
      ref WINDOWINFO pwi
  );

  public static void SetPosition(
      Window window,
      double x,
      double y
  )
  {
    window.WindowStartupLocation =
        WindowStartupLocation.Manual;

    window.SourceInitialized += (_, _) =>
    {
      IntPtr hwnd = new WindowInteropHelper(window).Handle;

      var info = new WINDOWINFO
      {
        cbSize = (uint)Marshal.SizeOf<WINDOWINFO>()
      };

      if (!GetWindowInfo(hwnd, ref info))
        return;

      double scale = VisualTreeHelper.GetDpi(window).DpiScaleX;

      double offsetX = info.rcClient.Left - info.rcWindow.Left;

      window.Left = x - (offsetX / scale);
      window.Top = y;
    };
  }
}