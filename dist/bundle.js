(() => {
  var __getOwnPropNames = Object.getOwnPropertyNames;
  var __commonJS = (cb, mod) => function __require() {
    return mod || (0, cb[__getOwnPropNames(cb)[0]])((mod = { exports: {} }).exports, mod), mod.exports;
  };

  // example/app.ts
  var require_app = __commonJS({
    "example/app.ts"() {
      var callbacks = globalThis.__vormirCallbacks;
      callbacks["hello"] = () => {
        console.log("Tombol berhasil diklik!");
      };
      var tree = {
        type: "View",
        props: {
          style: {
            flexDirection: "column",
            padding: 24,
            gap: 12
          }
        },
        children: [
          {
            type: "Text",
            props: {
              text: "Selamat datang di Vormir!",
              style: {
                fontSize: 28,
                color: "#222222"
              }
            },
            children: []
          },
          {
            type: "Text",
            props: {
              text: "Native UI menggunakan WPF Renderer."
            },
            children: []
          },
          {
            type: "Button",
            props: {
              title: "Klik Saya",
              onPress: "hello"
            },
            children: []
          }
        ]
      };
      globalThis.__vormirRender(JSON.stringify(tree));
    }
  });
  require_app();
})();
//# sourceMappingURL=bundle.js.map
