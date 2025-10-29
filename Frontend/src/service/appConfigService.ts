import type { ApiResponse } from "../models/ApiResponse";
import type { AppConfig } from "../models/AppConfig";

const appConfigProxy = window.chrome.webview.hostObjects.appConfigBridge;

async function getAppConfig(): Promise<ApiResponse<AppConfig>> {
  const serializedConfig = await appConfigProxy.GetAppConfig();
  const apiResponse: ApiResponse<AppConfig> = JSON.parse(serializedConfig);

  return apiResponse;
}

async function writeAppConfig(appConfig: AppConfig): Promise<void> {
  const serializedConfig = JSON.stringify(appConfig);
  await appConfigProxy.WriteAppConfig(serializedConfig);
}

export { getAppConfig, writeAppConfig };
