(() => {
  var __getOwnPropNames = Object.getOwnPropertyNames;
  var __esm = (fn, res) => function __init() {
    return fn && (res = (0, fn[__getOwnPropNames(fn)[0]])(fn = 0)), res;
  };
  var __commonJS = (cb, mod) => function __require() {
    return mod || (0, cb[__getOwnPropNames(cb)[0]])((mod = { exports: {} }).exports, mod), mod.exports;
  };

  // sdk/src/core/vnode.ts
  function createVNode(type, props = {}, children = []) {
    return {
      type,
      props: { ...props },
      children: children.filter(
        (child) => child !== null && child !== void 0 && child !== false
      )
    };
  }
  var init_vnode = __esm({
    "sdk/src/core/vnode.ts"() {
      "use strict";
    }
  });

  // sdk/src/components/Window.ts
  function Window(props) {
    if (typeof props.location === "object" && props.location.x === 0) {
      props.location.offsetX = -7;
    }
    return createVNode("Window", props, props.children ?? []);
  }
  var init_Window = __esm({
    "sdk/src/components/Window.ts"() {
      "use strict";
      init_vnode();
    }
  });

  // sdk/src/components/Stack.ts
  function Stack(props) {
    return createVNode("Stack", props, props.children ?? []);
  }
  var init_Stack = __esm({
    "sdk/src/components/Stack.ts"() {
      "use strict";
      init_vnode();
    }
  });

  // sdk/src/components/Text.ts
  function Text(props) {
    return createVNode("Text", props);
  }
  var init_Text = __esm({
    "sdk/src/components/Text.ts"() {
      "use strict";
      init_vnode();
    }
  });

  // sdk/src/components/Button.ts
  var init_Button = __esm({
    "sdk/src/components/Button.ts"() {
      "use strict";
      init_vnode();
    }
  });

  // sdk/src/components/Border.ts
  var init_Border = __esm({
    "sdk/src/components/Border.ts"() {
      "use strict";
      init_vnode();
    }
  });

  // sdk/src/components/ScrollView.ts
  var init_ScrollView = __esm({
    "sdk/src/components/ScrollView.ts"() {
      "use strict";
      init_vnode();
    }
  });

  // sdk/src/components/Spacer.ts
  var init_Spacer = __esm({
    "sdk/src/components/Spacer.ts"() {
      "use strict";
      init_vnode();
    }
  });

  // sdk/src/components/Divider.ts
  var init_Divider = __esm({
    "sdk/src/components/Divider.ts"() {
      "use strict";
      init_vnode();
    }
  });

  // sdk/src/components/Image.ts
  var init_Image = __esm({
    "sdk/src/components/Image.ts"() {
      "use strict";
      init_vnode();
    }
  });

  // sdk/src/components/Icon.ts
  var init_Icon = __esm({
    "sdk/src/components/Icon.ts"() {
      "use strict";
      init_vnode();
    }
  });

  // sdk/src/components/TextInput.ts
  function TextInput(props) {
    return createVNode("TextInput", props);
  }
  var init_TextInput = __esm({
    "sdk/src/components/TextInput.ts"() {
      "use strict";
      init_vnode();
    }
  });

  // sdk/src/components/CheckBox.ts
  function CheckBox(props) {
    return createVNode("CheckBox", props);
  }
  var init_CheckBox = __esm({
    "sdk/src/components/CheckBox.ts"() {
      "use strict";
      init_vnode();
    }
  });

  // sdk/src/core/renderer.ts
  function getCallbacks() {
    if (!globalThis.__vormirCallbacks) {
      globalThis.__vormirCallbacks = {};
    }
    return globalThis.__vormirCallbacks;
  }
  function registerCallback(callback) {
    const id = `vormir_callback_${++callbackCounter}`;
    getCallbacks()[id] = callback;
    return id;
  }
  function serializeVNode(node) {
    if (node === null || node === void 0 || typeof node === "boolean") {
      return null;
    }
    if (typeof node === "string" || typeof node === "number") {
      return {
        type: "Text",
        props: {
          text: String(node)
        },
        children: []
      };
    }
    if (!("type" in node) || !("props" in node)) {
      return null;
    }
    const props = {};
    for (const [key, value] of Object.entries(node.props ?? {})) {
      if (typeof value === "function") {
        if (key === "onPress" || key === "onClick" || key === "onChange" || key === "onSubmit") {
          const callback = value;
          if (node.type === "CheckBox" && key === "onChange") {
            props[key] = registerCallback((value2) => {
              callback(value2 === "true");
            });
          } else {
            props[key] = registerCallback(callback);
          }
        }
      } else {
        props[key] = value;
      }
    }
    return {
      type: node.type,
      props,
      children: (node.children ?? []).map((child) => serializeVNode(child)).filter((child) => child !== null)
    };
  }
  function render(node) {
    if (typeof globalThis.__vormirRender !== "function") {
      throw new Error("Vormir Native Bridge belum tersedia.");
    }
    const tree = serializeVNode(node);
    globalThis.__vormirRender(JSON.stringify(tree));
  }
  function runApp(app) {
    render(app());
  }
  var callbackCounter;
  var init_renderer = __esm({
    "sdk/src/core/renderer.ts"() {
      "use strict";
      callbackCounter = 0;
      globalThis.__vormirInvokeCallback = (id, ...args) => {
        const callback = globalThis.__vormirCallbacks?.[id];
        if (typeof callback === "function") {
          callback(...args);
        }
      };
    }
  });

  // sdk/src/index.ts
  var init_src = __esm({
    "sdk/src/index.ts"() {
      "use strict";
      init_Window();
      init_Stack();
      init_Text();
      init_Button();
      init_Border();
      init_ScrollView();
      init_Spacer();
      init_Divider();
      init_Image();
      init_Icon();
      init_TextInput();
      init_CheckBox();
      init_vnode();
      init_renderer();
    }
  });

  // example/app.ts
  var require_app = __commonJS({
    "example/app.ts"() {
      init_src();
      function Main() {
        return Window({
          title: "CheckBox Example",
          width: 500,
          height: 400,
          children: [
            Stack({
              orientation: "vertical",
              spacing: 15,
              padding: 20,
              children: [
                Text({
                  text: "Pengaturan",
                  fontSize: 22
                }),
                TextInput({
                  placeholder: "test",
                  onChange: (v) => console.log(v)
                }),
                CheckBox({
                  label: "Ingat saya",
                  checked: true,
                  onChange: (value) => {
                    console.log("Ingat saya:", value);
                  }
                }),
                CheckBox({
                  label: "Aktifkan notifikasi",
                  checked: false,
                  onChange: (value) => {
                    console.log("Notifikasi:", value);
                  }
                }),
                CheckBox({
                  label: "Mode gelap",
                  checked: false,
                  color: "#2563eb",
                  fontSize: 16,
                  onChange: (value) => {
                    console.log("Mode gelap:", value);
                  }
                })
              ]
            })
          ]
        });
      }
      runApp(Main);
    }
  });
  require_app();
})();
//# sourceMappingURL=bundle.js.map
