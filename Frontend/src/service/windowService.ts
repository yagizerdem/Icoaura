import type { ApiResponse } from "../models/ApiResponse";

const windowProxy = window.chrome.webview.hostObjects.windowBridge;

async function rebootAsAdmin() {
  await windowProxy.RebootAsAdmin();
}

async function HasAdminPrivilege(): Promise<boolean> {
  return await windowProxy.HasAdminPrivilege();
}

export { rebootAsAdmin, HasAdminPrivilege };
