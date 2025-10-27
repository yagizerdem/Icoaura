import type { ApiResponse } from "../models/ApiResponse";
import type { PackConfig } from "../models/PackConfig";

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

export { getAllPackConfigs, createPack };
