import { Window, Stack, Text, TimePicker, runApp } from "@vormir/sdk";

function Main() {
  return Window({
    title: "TimePicker Example",
    width: 500,
    height: 350,
    children: [
      Stack({
        orientation: "vertical",
        spacing: 15,
        padding: 20,
        children: [
          Text({
            text: "Pilih Waktu",
            fontSize: 20,
          }),

          TimePicker({
            width: 200,
            value: "14:30",
            minTime: "08:00",
            maxTime: "22:00",
            visible: true,
            placeholder: "HH:mm",
            onChange: (value) => {
              console.log("Waktu:", value);
            },
          }),
        ],
      }),
    ],
  });
}

runApp(Main);
