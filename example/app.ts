import { Window, Stack, Grid, Border, Text, runApp } from "@vormir/sdk";

function Main() {
  return Window({
    title: "Grid Example",
    width: 700,
    height: 500,
    children: [
      Stack({
        orientation: "vertical",
        spacing: 15,
        padding: 20,
        children: [
          Text({
            text: "Dashboard",
            fontSize: 24,
            fontWeight: "bold",
          }),

          Grid({
            columns: 3,
            spacing: 12,
            children: [
              Border({
                background: "#dbeafe",
                padding: 20,
                children: [Text({ text: "Item 1" })],
              }),

              Border({
                background: "#dcfce7",
                padding: 20,
                children: [Text({ text: "Item 2" })],
              }),

              Border({
                background: "#fef3c7",
                padding: 20,
                children: [Text({ text: "Item 3" })],
              }),

              Border({
                background: "#fce7f3",
                padding: 20,
                children: [Text({ text: "Item 4" })],
              }),

              Border({
                background: "#ede9fe",
                padding: 20,
                children: [Text({ text: "Item 5" })],
              }),

              Border({
                background: "#cffafe",
                padding: 20,
                children: [Text({ text: "Item 6" })],
              }),
            ],
          }),
        ],
      }),
    ],
  });
}

runApp(Main);
