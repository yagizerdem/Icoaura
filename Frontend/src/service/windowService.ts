import type { Theme } from "../enum/Theme";
import type { ApiResponse } from "../models/ApiResponse";

const windowProxy = window.chrome.webview.hostObjects.windowBridge;

async function rebootAsAdmin() {
  await windowProxy.RebootAsAdmin();
}

async function HasAdminPrivilege(): Promise<boolean> {
  return await windowProxy.HasAdminPrivilege();
}

async function GetSystemDefaultTheme(): Promise<Theme.Dark | Theme.Light> {
  const response = await windowProxy.GetSystemDefaultTheme();
  return response;
}

export { rebootAsAdmin, HasAdminPrivilege, GetSystemDefaultTheme };
