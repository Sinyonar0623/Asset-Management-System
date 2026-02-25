import fs from "fs";
import path from "path";
const b = String.raw\;
console.log("base dir:", b);
fs.writeFileSync(path.join(b,"borrow","page.tsx"), "// placeholder", "utf8");
console.log("ok");
