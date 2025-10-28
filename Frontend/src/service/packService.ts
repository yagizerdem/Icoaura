import type { ApiResponse } from "../models/ApiResponse";
import type { PackConfig } from "../models/PackConfig";
import type { PackItem } from "../models/PackItem";

const packProx = window.chrome.webview.hostObjects.packBridge;

async function getAllPackConfigs(): Promise<ApiResponse<PackConfig[]>> {
  const serializedResponse = await packProx.GetAllPackConfigs();
  const apiResponse: ApiResponse<PackConfig[]> = JSON.parse(serializedResponse);
  return apiResponse;
}

async function createPack(packConfig: PackConfig): Promise<ApiResponse<null>> {
  const serializedResponse = await packProx.CreatePack(
    JSON.stringify(packConfig)
  );
  const apiResponse: ApiResponse<null> = JSON.parse(serializedResponse);
  return apiResponse;
}

async function DeletePack(
  packId: string,
  deleteIcons: boolean
): Promise<ApiResponse<null>> {
  var serializedResponse = await packProx.DeletePack(packId, deleteIcons);
  const apiResponse: ApiResponse<null> = JSON.parse(serializedResponse);
  return apiResponse;
}

async function WritePackConfig(
  packConfig: PackConfig
): Promise<ApiResponse<PackConfig>> {
  const serializedResponse = await packProx.WritePackConfig(
    JSON.stringify(packConfig)
  );
  const apiResponse: ApiResponse<PackConfig> = JSON.parse(serializedResponse);
  return apiResponse;
}

async function GetPackItems(packId: string): Promise<ApiResponse<PackItem[]>> {
  const serializedResponse = await packProx.GetPackItems(packId);
  const apiResponse: ApiResponse<PackItem[]> = JSON.parse(serializedResponse);
  return apiResponse;
}

async function GetPackItemIconBase64(
  packId: string,
  itemId: string
): Promise<ApiResponse<string | null>> {
  const serializedResponse = await packProx.GetPackItemIconBase64(
    packId,
    itemId
  );
  const apiResponse: ApiResponse<string | null> =
    JSON.parse(serializedResponse);
  return apiResponse;
}

async function AddDesktopIcons(
  packId: string
): Promise<ApiResponse<PackItem[]>> {
  const serializedResponse = await packProx.AddDesktopIcons(packId);
  const apiResponse: ApiResponse<PackItem[]> = JSON.parse(serializedResponse);
  return apiResponse;
}

async function AppendPackItemFromPath(
  packId: string,
  filePath: string
): Promise<ApiResponse<PackItem>> {
  const serializedResponse = await packProx.AppendPackItemFromPath(
    packId,
    filePath
  );
  const apiResponse: ApiResponse<PackItem> = JSON.parse(serializedResponse);
  return apiResponse;
}

export {
  getAllPackConfigs,
  createPack,
  DeletePack,
  WritePackConfig,
  GetPackItems,
  GetPackItemIconBase64,
  AddDesktopIcons,
  AppendPackItemFromPath,
};
