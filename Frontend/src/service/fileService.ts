import type { ApiResponse } from "../models/ApiResponse";
import type { DirMetaData } from "../models/DirMetaData";
import type { LnkMetaData } from "../models/LnkMetaData";
import type { UrlMetaData } from "../models/UrlMetaData";

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

async function ImportPack(path: string): Promise<ApiResponse<void>> {
  const serialized = await fileProx.ImportPack(path);
  return JSON.parse(serialized) as ApiResponse<void>;
}

async function ExportPack(packId: string): Promise<ApiResponse<string>> {
  const serialized = await fileProx.ExportPack(packId);
  return JSON.parse(serialized) as ApiResponse<string>;
}

async function GetLnkMetaData(
  filePath: string
): Promise<ApiResponse<LnkMetaData>> {
  const serialized = await fileProx.GetLnkMetaData(filePath);
  return JSON.parse(serialized) as ApiResponse<LnkMetaData>;
}
async function GetUrlMetaData(
  filePath: string
): Promise<ApiResponse<UrlMetaData>> {
  const serialized = await fileProx.GetUrlMetaData(filePath);
  return JSON.parse(serialized) as ApiResponse<any>;
}

async function GetDirMetaData(
  filePath: string
): Promise<ApiResponse<DirMetaData>> {
  const serialized = await fileProx.GetDirMetaData(filePath);
  return JSON.parse(serialized) as ApiResponse<DirMetaData>;
}

export {
  selectFileAbsolutePath,
  getBase64FromPath,
  selectDirectoryPath,
  SelectFileRelativeFilePath,
  SelectRelativeDirectoryPath,
  IsFileSystemEntryExist,
  ImportPack,
  ExportPack,
  GetLnkMetaData,
  GetUrlMetaData,
  GetDirMetaData,
};
