import { Window, Stack, Text, Button, Spacer, runApp } from "@vormir/sdk";

function Main() {
  return Window({
    title: "Spacer Example",
    width: 500,
    height: 400,
    children: [
      Stack({
        orientation: "vertical",
        padding: 20,
        children: [
          Text({ text: "Header" }),

          Spacer(),
          Button({ content: "Simpan" }),
        ],
      }),
    ],
  });
}

runApp(Main);
