import { Window, Stack, Text, Dropdown, runApp } from "@vormir/sdk";

function Main() {
  return Window({
    title: "Dropdown Example",
    width: 500,
    height: 350,
    children: [
      Stack({
        orientation: "vertical",
        spacing: 15,
        padding: 20,
        children: [
          Text({
            text: "Pilih Negara",
            fontSize: 20,
          }),

          Dropdown({
            width: 250,
            placeholder: "Pilih negara...",
            options: [
              { label: "Indonesia", value: "id" },
              { label: "Malaysia", value: "my" },
              { label: "Singapore", value: "sg" },
              { label: "Thailand", value: "th" },
            ],
            onChange: (value) => {
              console.log("Negara:", value);
            },
          }),
        ],
      }),
    ],
  });
}

runApp(Main);
