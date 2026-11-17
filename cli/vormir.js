#!/usr/bin/env node

const path = require("node:path");
const fs = require("node:fs/promises");
const { spawn, execFile } = require("node:child_process");
const { createRequire } = require("node:module");

// ============================================================
// CONFIGURATION
// ============================================================

const ROOT = path.resolve(__dirname, "..");

const SDK_DIR = path.join(ROOT, "sdk");
const SDK_PACKAGE = path.join(SDK_DIR, "package.json");

const EXAMPLE_FILE = path.join(ROOT, "example", "app.ts");

const DIST_DIR = path.join(ROOT, "dist");
const BUNDLE_FILE = path.join(DIST_DIR, "bundle.js");

const RUNTIME_DIR = path.join(ROOT, "runtime");
const RUNTIME_PROJECT = path.join(RUNTIME_DIR, "Vormir.csproj");

const RUNTIME_OUTPUT_DIR = path.join(
  RUNTIME_DIR,
  "bin",
  "Debug",
  "net8.0-windows",
);

const RUNTIME_BUNDLE_DIR = path.join(RUNTIME_OUTPUT_DIR, "dist");

const RUNTIME_BUNDLE_FILE = path.join(RUNTIME_BUNDLE_DIR, "bundle.js");

const requireFromSdk = createRequire(SDK_PACKAGE);
const esbuild = requireFromSdk("esbuild");

// ============================================================
// STATE
// ============================================================

let runtimeProcess = null;
let esbuildContext = null;
let isStopping = false;
let restartQueue = Promise.resolve();

// ============================================================
// LOGGING
// ============================================================

function log(message) {
  console.log(`\x1b[36m[Vormir]\x1b[0m ${message}`);
}

function success(message) {
  console.log(`\x1b[32m[Vormir]\x1b[0m ${message}`);
}

function warning(message) {
  console.log(`\x1b[33m[Vormir]\x1b[0m ${message}`);
}

function error(message) {
  console.error(`\x1b[31m[Vormir]\x1b[0m ${message}`);
}

// ============================================================
// PROCESS
// ============================================================

function runCommand(command, args, options = {}) {
  return new Promise((resolve, reject) => {
    const child = spawn(command, args, {
      cwd: ROOT,
      stdio: "inherit",
      shell: false,
      ...options,
    });

    child.once("error", reject);

    child.once("exit", (code) => {
      if (code === 0) {
        resolve();
      } else {
        reject(new Error(`${command} gagal dengan exit code ${code}`));
      }
    });
  });
}

// ============================================================
// TYPESCRIPT BUILD
// ============================================================

async function buildBundle() {
  log("Building TypeScript...");

  await fs.mkdir(DIST_DIR, {
    recursive: true,
  });

  await esbuild.build({
    absWorkingDir: ROOT,

    entryPoints: [EXAMPLE_FILE],

    bundle: true,
    platform: "browser",
    format: "iife",
    target: "es2020",

    sourcemap: true,

    outfile: BUNDLE_FILE,

    alias: {
      "@vormir/sdk": path.join(SDK_DIR, "src", "index.ts"),
    },

    logLevel: "info",
  });

  success("TypeScript build berhasil.");
}

// ============================================================
// WPF BUILD
// ============================================================

async function buildRuntime() {
  log("Building .NET WPF Runtime...");

  await runCommand("dotnet", [
    "build",
    RUNTIME_PROJECT,
    "--configuration",
    "Debug",
  ]);

  success("WPF Runtime build berhasil.");
}

// ============================================================
// BUNDLE SYNC
// ============================================================

async function syncBundle() {
  await fs.access(BUNDLE_FILE);

  await fs.mkdir(RUNTIME_BUNDLE_DIR, {
    recursive: true,
  });

  await fs.copyFile(BUNDLE_FILE, RUNTIME_BUNDLE_FILE);

  success("Bundle berhasil disinkronkan ke WPF Runtime.");
}

// ============================================================
// RUNTIME START
// ============================================================

function startRuntime() {
  if (isStopping) {
    return null;
  }

  log("Starting Vormir Application...");

  const child = spawn(
    "dotnet",
    [
      "run",
      "--no-build",
      "--no-restore",
      "--configuration",
      "Debug",
      "--project",
      RUNTIME_PROJECT,
    ],
    {
      cwd: ROOT,
      stdio: "inherit",
      shell: false,
      windowsHide: false,
    },
  );

  runtimeProcess = child;

  child.once("error", (err) => {
    error(`Runtime gagal dijalankan: ${err.message}`);
  });

  child.once("exit", (code) => {
    if (runtimeProcess === child) {
      runtimeProcess = null;
    }

    if (!isStopping) {
      warning(`Runtime berhenti. Exit code: ${code ?? "unknown"}`);
    }
  });

  return child;
}

// ============================================================
// RUNTIME STOP
// ============================================================

async function stopRuntime() {
  const child = runtimeProcess;

  if (!child || !child.pid) {
    runtimeProcess = null;
    return;
  }

  runtimeProcess = null;

  log("Stopping previous runtime...");

  if (process.platform === "win32") {
    await new Promise((resolve) => {
      execFile("taskkill", ["/PID", String(child.pid), "/T", "/F"], () =>
        resolve(),
      );
    });

    return;
  }

  child.kill("SIGTERM");

  await new Promise((resolve) => {
    const timeout = setTimeout(resolve, 3000);

    child.once("exit", () => {
      clearTimeout(timeout);
      resolve();
    });
  });
}

// ============================================================
// RESTART RUNTIME
// ============================================================

async function restartRuntime() {
  if (isStopping) {
    return;
  }

  await syncBundle();

  if (runtimeProcess) {
    await stopRuntime();
  }

  if (!isStopping) {
    startRuntime();
  }
}

// ============================================================
// BUILD COMMAND
// ============================================================

async function build() {
  log("Starting production build...");

  await buildBundle();

  await buildRuntime();

  await syncBundle();

  success("Vormir build selesai.");

  console.log("");
  console.log("Output:");
  console.log(`  Bundle : ${BUNDLE_FILE}`);
  console.log(`  Runtime: ${RUNTIME_OUTPUT_DIR}`);
  console.log("");
}

// ============================================================
// RUN COMMAND
// ============================================================

async function run() {
  await build();

  const child = startRuntime();

  if (!child) {
    return;
  }

  await new Promise((resolve, reject) => {
    child.once("error", reject);
    child.once("exit", (code) => resolve(code ?? 0));
  });
}

// ============================================================
// DEVELOPMENT MODE
// ============================================================

async function dev() {
  log("Starting development mode...");

  // Build WPF satu kali.
  await buildRuntime();

  // Buat context esbuild.
  esbuildContext = await esbuild.context({
    absWorkingDir: ROOT,

    entryPoints: [EXAMPLE_FILE],

    bundle: true,
    platform: "browser",
    format: "iife",
    target: "es2020",

    sourcemap: true,

    outfile: BUNDLE_FILE,

    alias: {
      "@vormir/sdk": path.join(SDK_DIR, "src", "index.ts"),
    },

    logLevel: "info",

    plugins: [
      {
        name: "vormir-runtime",

        setup(build) {
          build.onEnd(async (result) => {
            if (isStopping) {
              return;
            }

            if (result.errors.length > 0) {
              warning("Build gagal. Runtime tidak direstart.");

              return;
            }

            // Antrekan restart agar tidak tumpang tindih.
            restartQueue = restartQueue.then(async () => {
              if (!isStopping) {
                await restartRuntime();
              }
            });

            await restartQueue;
          });
        },
      },
    ],
  });

  // Daftarkan watcher.
  await esbuildContext.watch();

  log("Watching TypeScript dependencies...");
  log("Edit file di folder sdk/src atau example.");
  log("Tekan Ctrl+C untuk berhenti.");
}

// ============================================================
// CLEAN COMMAND
// ============================================================

async function clean() {
  log("Cleaning project...");

  await stopRuntime();

  const folders = [
    DIST_DIR,
    path.join(RUNTIME_DIR, "bin"),
    path.join(RUNTIME_DIR, "obj"),
  ];

  for (const folder of folders) {
    await fs.rm(folder, {
      recursive: true,
      force: true,
    });
  }

  success("Clean selesai.");
}

// ============================================================
// SHUTDOWN
// ============================================================

async function shutdown() {
  if (isStopping) {
    return;
  }

  isStopping = true;

  log("Shutting down Vormir CLI...");

  try {
    if (esbuildContext) {
      await esbuildContext.dispose();
      esbuildContext = null;
    }

    await restartQueue;

    await stopRuntime();
  } catch (err) {
    error(err.message);
  }

  process.exit(0);
}

// ============================================================
// HELP
// ============================================================

function help() {
  console.log(`
Vormir CLI
Native Desktop Application Framework

Usage:
  vormir <command>

Commands:

  dev       Start development mode
  build     Build application
  run       Build and run application
  clean     Remove generated files
  help      Show help

NPM Commands:

  npm run dev
  npm run build
  npm start
  npm run clean
`);
}

// ============================================================
// MAIN
// ============================================================

async function main() {
  const command = process.argv[2] || "help";

  process.once("SIGINT", shutdown);
  process.once("SIGTERM", shutdown);

  try {
    switch (command) {
      case "dev":
        await dev();
        break;

      case "build":
        await build();
        break;

      case "run":
      case "start":
        await run();
        break;

      case "clean":
        await clean();
        break;

      case "help":
      case "--help":
      case "-h":
        help();
        break;

      default:
        error(`Command tidak dikenal: ${command}`);
        help();
        process.exitCode = 1;
        break;
    }
  } catch (err) {
    error(err.stack || err.message);
    process.exitCode = 1;
  }
}

main();
