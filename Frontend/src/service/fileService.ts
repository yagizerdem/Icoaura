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

async function selectDirectoryPath(): Promise<string> {
  const dirPath = await fileProx.SelectDirectoryPath();
  return dirPath;
}

async function SelectFileRelativeFilePath(
  allowedExtensions: string[]
): Promise<string> {
  const filePath = await fileProx.SelectFileRelativeFilePath(
    JSON.stringify(allowedExtensions)
  );
  return filePath;
}

async function SelectRelativeDirectoryPath(): Promise<string> {
  const dirPath = await fileProx.SelectRelativeDirectoryPath();
  return dirPath;
}

async function IsFileSystemEntryExist(filePath: string): Promise<boolean> {
  const response = await fileProx.IsFileSystemEntryExist(filePath);
  return response;
}

export {
  selectFileAbsolutePath,
  getBase64FromPath,
  selectDirectoryPath,
  SelectFileRelativeFilePath,
  SelectRelativeDirectoryPath,
  IsFileSystemEntryExist,
};
