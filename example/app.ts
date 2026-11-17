import { Window, Stack, Text, Divider, runApp } from "@vormir/sdk";

function Main() {
  return Window({
    title: "Divider Example",
    width: 500,
    height: 400,
    children: [
      Stack({
        orientation: "vertical",
        spacing: 10,
        padding: 20,
        children: [
          Text({ text: "Profil" }),

          Divider({
            color: "#cccccc",
            thickness: 1,
          }),

          Text({ text: "Pengaturan" }),

          Divider({
            color: "#ff0000",
            thickness: 2,
          }),
        ],
      }),
    ],
  });
}

runApp(Main);
