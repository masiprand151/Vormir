import { Window, Stack, Text, TextInput, runApp } from "@vormir/sdk";

function Main() {
  return Window({
    title: "TextInput Example",
    width: 500,
    height: 400,
    children: [
      Stack({
        orientation: "vertical",
        spacing: 12,
        padding: 20,
        children: [
          Text({
            text: "Nama pengguna",
            fontSize: 16,
          }),

          TextInput({
            placeholder: "Masukkan nama...",
            value: "",
            width: 300,
            height: 42,
            fontSize: 14,
            color: "#222222",
            background: "#ffffff",
            borderColor: "#cccccc",
            borderWidth: 1,
            borderRadius: 6,
            padding: 10,

            onChange: (value) => {
              console.log("Nama:", value);
            },
          }),

          Text({
            text: "Silakan masukkan nama kamu.",
            fontSize: 12,
          }),
        ],
      }),
    ],
  });
}

runApp(Main);
