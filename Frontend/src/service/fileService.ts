import type { ApiResponse } from "../models/ApiResponse";

const fileProx = window.chrome.webview.hostObjects.fileBridge;

async function selectFileAbsolutePath(
  allowedExtensions: string[]
): Promise<string> {
  const filePath = await fileProx.SelectFilePath(
    JSON.stringify(allowedExtensions)
  );
  return filePath;
}

async function getBase64FromPath(
  filePath: string
): Promise<ApiResponse<string>> {
  const serialized = await fileProx.GetBase64FromPath(filePath);
  return JSON.parse(serialized) as ApiResponse<string>;
}

export { selectFileAbsolutePath, getBase64FromPath };
