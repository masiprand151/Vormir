
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Vormir.Core;
using Vormir.UI;

namespace Vormir;

public partial class App : Application
{
  private JavaScriptRuntime? _javascriptRuntime;

  protected override void OnStartup(StartupEventArgs e)
  {
    base.OnStartup(e);

    var window = new Window
    {
      Title = "Vormir",
      Width = 800,
      Height = 600,
      Background = Brushes.White,
      WindowStartupLocation = WindowStartupLocation.CenterScreen
    };

    MainWindow = window;

    var bridge = new WpfNativeBridge(
        window,
        callbackId =>
        {
          _javascriptRuntime?.InvokeCallback(callbackId);
        }
    );

    _javascriptRuntime = new JavaScriptRuntime(
        // JavaScript console.log
        message =>
        {
          Console.WriteLine("[Vormir JS] " + message);
        },

        // JavaScript native render
        json =>
        {
          Dispatcher.Invoke(() =>
              {
                bridge.Render(json);
              });
        }
    );

    window.Show();

    try
    {
      string bundlePath = Path.Combine(
          AppContext.BaseDirectory,
          "dist",
          "bundle.js"
      );

      _javascriptRuntime.ExecuteFile(bundlePath);
    }
    catch (Exception ex)
    {
      window.Content = new TextBlock
      {
        Text = ex.ToString(),
        Foreground = Brushes.Red,
        TextWrapping = TextWrapping.Wrap,
        Margin = new Thickness(20)
      };

      Console.WriteLine(ex);
    }
  }
}
