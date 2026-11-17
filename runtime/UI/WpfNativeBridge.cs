
using System;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Vormir.UI;

public sealed class WpfNativeBridge
{
  private readonly Window _window;
  private readonly Action<string> _invokeCallback;

  public WpfNativeBridge(
      Window window,
      Action<string> invokeCallback)
  {
    _window = window;
    _invokeCallback = invokeCallback;
  }

  public void Render(string json)
  {
    using var document = JsonDocument.Parse(json);

    FrameworkElement element = BuildElement(document.RootElement);

    _window.Content = element;
  }

  private FrameworkElement BuildElement(JsonElement node)
  {
    string type = GetString(node, "type") ?? "View";

    JsonElement props = GetProperty(node, "props");
    JsonElement children = GetProperty(node, "children");

    return type switch
    {
      "View" => BuildView(props, children),
      "Text" => BuildText(props),
      "Button" => BuildButton(props),

      _ => new TextBlock
      {
        Text = $"Unknown component: {type}",
        Foreground = Brushes.Red
      }
    };
  }

  private FrameworkElement BuildView(
      JsonElement props,
      JsonElement children)
  {
    string direction =
        GetStyleString(props, "flexDirection") ?? "column";

    var panel = new StackPanel
    {
      Orientation = direction == "row"
            ? Orientation.Horizontal
            : Orientation.Vertical
    };

    if (children.ValueKind == JsonValueKind.Array)
    {
      foreach (var child in children.EnumerateArray())
      {
        panel.Children.Add(BuildElement(child));
      }
    }

    ApplySpacing(panel, props);

    return panel;
  }

  private FrameworkElement BuildText(JsonElement props)
  {
    string value =
        GetString(props, "text") ??
        GetString(props, "children") ??
        "";

    return new TextBlock
    {
      Text = value,
      FontSize = GetStyleDouble(props, "fontSize") ?? 16,
      Foreground = GetBrush(props, "color") ?? Brushes.Black,
      TextWrapping = TextWrapping.Wrap
    };
  }

  private FrameworkElement BuildButton(JsonElement props)
  {
    string title =
        GetString(props, "title") ??
        GetString(props, "text") ??
        "Button";

    string? callbackId = GetString(props, "onPress");

    var button = new Button
    {
      Content = title,
      Padding = new Thickness(12, 7, 12, 7),
      Margin = new Thickness(4),
      FontSize = GetStyleDouble(props, "fontSize") ?? 14
    };

    if (!string.IsNullOrWhiteSpace(callbackId))
    {
      button.Click += (_, _) =>
      {
        _invokeCallback(callbackId);
      };
    }

    return button;
  }

  private static void ApplySpacing(
      StackPanel panel,
      JsonElement props)
  {
    double padding = GetStyleDouble(props, "padding") ?? 0;
    double gap = GetStyleDouble(props, "gap") ?? 0;

    panel.Margin = new Thickness(padding);

    foreach (UIElement child in panel.Children)
    {
      if (child is FrameworkElement element)
      {
        element.Margin = new Thickness(gap / 2);
      }
    }
  }

  private static JsonElement GetProperty(
      JsonElement element,
      string name)
  {
    if (element.ValueKind == JsonValueKind.Object &&
        element.TryGetProperty(name, out var value))
    {
      return value;
    }

    return default;
  }

  private static string? GetString(
      JsonElement element,
      string name)
  {
    var value = GetProperty(element, name);

    return value.ValueKind == JsonValueKind.String
        ? value.GetString()
        : null;
  }

  private static string? GetStyleString(
      JsonElement props,
      string name)
  {
    var style = GetProperty(props, "style");

    return GetString(style, name);
  }

  private static double? GetStyleDouble(
      JsonElement props,
      string name)
  {
    var style = GetProperty(props, "style");
    var value = GetProperty(style, name);

    if (value.ValueKind == JsonValueKind.Number &&
        value.TryGetDouble(out double result))
    {
      return result;
    }

    return null;
  }

  private static Brush? GetBrush(
      JsonElement props,
      string name)
  {
    string? color = GetStyleString(props, name);

    if (string.IsNullOrWhiteSpace(color))
      return null;

    try
    {
      return new BrushConverter()
          .ConvertFromString(color) as Brush;
    }
    catch
    {
      return null;
    }
  }
}
