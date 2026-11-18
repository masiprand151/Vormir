import { Window, Stack, Text, CheckBox, runApp, TextInput } from "@vormir/sdk";

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
            fontSize: 22,
          }),

          TextInput({
            placeholder: "test",
            onChange: (v) => console.log(v),
          }),

          CheckBox({
            label: "Ingat saya",
            checked: true,
            onChange: (value) => {
              console.log("Ingat saya:", value);
            },
          }),

          CheckBox({
            label: "Aktifkan notifikasi",
            checked: false,
            onChange: (value) => {
              console.log("Notifikasi:", value);
            },
          }),

          CheckBox({
            label: "Mode gelap",
            checked: false,
            color: "#2563eb",
            fontSize: 16,
            onChange: (value) => {
              console.log("Mode gelap:", value);
            },
          }),
        ],
      }),
    ],
  });
}

runApp(Main);
