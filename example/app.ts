import { Window, Stack, Text, Switch, runApp } from "@vormir/sdk";

function Main() {
  return Window({
    title: "Switch Example",
    width: 500,
    height: 400,
    children: [
      Stack({
        orientation: "vertical",
        spacing: 20,
        padding: 20,
        children: [
          Text({
            text: "Pengaturan",
            fontSize: 24,
            fontWeight: "bold",
          }),

          Stack({
            orientation: "horizontal",
            spacing: 12,
            children: [
              Text({
                text: "Notifikasi",
                fontSize: 16,
              }),

              Switch({
                checked: true,
                color: "#2563eb",
                onChange: (value) => {
                  console.log("Notifikasi:", value);
                },
              }),
            ],
          }),

          Stack({
            orientation: "horizontal",
            spacing: 12,
            children: [
              Text({
                text: "Mode gelap",
                fontSize: 16,
              }),

              Switch({
                checked: false,
                color: "#16a34a",
                onChange: (value) => {
                  console.log("Mode gelap:", value);
                },
              }),
            ],
          }),
        ],
      }),
    ],
  });
}

runApp(Main);
