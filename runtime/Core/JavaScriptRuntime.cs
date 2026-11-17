
using System;
using System.IO;
using Jint;

namespace Vormir.Core;

public sealed class JavaScriptRuntime
{
  private readonly Engine _engine;

  public JavaScriptRuntime(
      Action<string> onLog,
      Action<string> onRender)
  {
    _engine = new Engine(options =>
    {
      options.ExperimentalFeatures =
              ExperimentalFeature.TaskInterop;
    });

    _engine.SetValue(
        "__vormirLog",
        new Action<string>(onLog)
    );

    _engine.SetValue(
        "__vormirRender",
        new Action<string>(onRender)
    );

    _engine.Execute("""
            globalThis.console = {
                log: (...args) => {
                    __vormirLog(args.map(String).join(" "));
                },
                warn: (...args) => {
                    __vormirLog("[WARN] " + args.map(String).join(" "));
                },
                error: (...args) => {
                    __vormirLog("[ERROR] " + args.map(String).join(" "));
                }
            };

            globalThis.__vormirCallbacks = Object.create(null);

            globalThis.__vormirInvokeCallback = function(id) {
                const callback = globalThis.__vormirCallbacks[id];

                if (typeof callback === "function") {
                    callback();
                }
            };
        """);
  }

  public void ExecuteFile(string filePath)
  {
    if (!File.Exists(filePath))
    {
      throw new FileNotFoundException(
          "JavaScript bundle tidak ditemukan.",
          filePath
      );
    }

    string source = File.ReadAllText(filePath);

    _engine.Execute(source);
  }

  public void Execute(string source)
  {
    _engine.Execute(source);
  }

  public void InvokeCallback(string callbackId)
  {
    _engine.Invoke(
        "__vormirInvokeCallback",
        callbackId
    );
  }
}
