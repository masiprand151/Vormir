import { Window, Stack, Text, RadioButton, Divider, runApp } from "@vormir/sdk";

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
            fontWeight: "bold",
          }),

          RadioButton({
            label: "Pria",
            group: "gender",
            value: "male",
            checked: true,
            onChange: (value) => {
              console.log("Gender:", value);
            },
          }),

          RadioButton({
            label: "Wanita",
            group: "gender",
            value: "female",
            onChange: (value) => {
              console.log("Gender:", value);
            },
          }),

          Divider({
            thickness: 1,
            color: "#dddddd",
          }),

          Text({
            text: "Pilih metode pembayaran",
            fontSize: 20,
            fontWeight: "bold",
          }),

          RadioButton({
            label: "Cash",
            group: "payment",
            value: "cash",
            checked: true,
            onChange: (value) => {
              console.log("Payment:", value);
            },
          }),

          RadioButton({
            label: "Transfer",
            group: "payment",
            value: "transfer",
            onChange: (value) => {
              console.log("Payment:", value);
            },
          }),

          RadioButton({
            label: "QRIS",
            group: "payment",
            value: "qris",
            onChange: (value) => {
              console.log("Payment:", value);
            },
          }),
        ],
      }),
    ],
  });
}

runApp(Main);
