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
  function Divider(props = {}) {
    return createVNode("Divider", props);
  }
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
  var init_TextInput = __esm({
    "sdk/src/components/TextInput.ts"() {
      "use strict";
      init_vnode();
    }
  });

  // sdk/src/components/CheckBox.ts
  var init_CheckBox = __esm({
    "sdk/src/components/CheckBox.ts"() {
      "use strict";
      init_vnode();
    }
  });

  // sdk/src/components/RadioButton.ts
  function RadioButton(props) {
    return createVNode("RadioButton", props);
  }
  var init_RadioButton = __esm({
    "sdk/src/components/RadioButton.ts"() {
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
      init_RadioButton();
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
          title: "RadioButton Example",
          width: 500,
          height: 400,
          children: [
            Stack({
              orientation: "vertical",
              spacing: 12,
              padding: 20,
              children: [
                Text({
                  text: "Pilih jenis kelamin",
                  fontSize: 20,
                  fontWeight: "bold"
                }),
                RadioButton({
                  label: "Pria",
                  group: "gender",
                  value: "male",
                  checked: true,
                  onChange: (value) => {
                    console.log("Gender:", value);
                  }
                }),
                RadioButton({
                  label: "Wanita",
                  group: "gender",
                  value: "female",
                  onChange: (value) => {
                    console.log("Gender:", value);
                  }
                }),
                Divider({
                  thickness: 1,
                  color: "#dddddd"
                }),
                Text({
                  text: "Pilih metode pembayaran",
                  fontSize: 20,
                  fontWeight: "bold"
                }),
                RadioButton({
                  label: "Cash",
                  group: "payment",
                  value: "cash",
                  checked: true,
                  onChange: (value) => {
                    console.log("Payment:", value);
                  }
                }),
                RadioButton({
                  label: "Transfer",
                  group: "payment",
                  value: "transfer",
                  onChange: (value) => {
                    console.log("Payment:", value);
                  }
                }),
                RadioButton({
                  label: "QRIS",
                  group: "payment",
                  value: "qris",
                  onChange: (value) => {
                    console.log("Payment:", value);
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
