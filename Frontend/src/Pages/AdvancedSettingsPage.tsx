import type { ApiResponse } from "../models/ApiResponse";
import type { AppConfig } from "../models/AppConfig";
import { useAppContext } from "../Providers/AppContext";
import { exportAppConfig, importAppConfig } from "../service/appConfigService";
import { selectFileAbsolutePath } from "../service/fileService";
import { ModernButton } from "../ui/ModernButton";
import { ModernCheckButton } from "../ui/ModernCheckButton";
import { ModernNumberInput } from "../ui/ModernNumberInput";
import { ModernSwitch } from "../ui/ModernSwitch";
import { ModernTextInput } from "../ui/ModernTextInput";
import { flash } from "../util/cameraFlash";
import { showToast, Toast } from "../util/toast";

function AdvancedSettingsPage() {
  const { appConfig, setAppConfig, setIsLoading } = useAppContext();

  function handleOnEnableLoggingChange(checked: boolean) {
    if (appConfig) {
      setAppConfig({
        ...appConfig,
        EnableLogging: checked,
      });
    }
  }

  function handleOnEnableTraceLoggingChange(checked: boolean) {
    if (appConfig) {
      setAppConfig({
        ...appConfig,
        EnableTraceLogging: checked,
      });
    }
  }

  function handleOnEnableDebugLoggingChange(checked: boolean) {
    if (appConfig) {
      setAppConfig({
        ...appConfig,
        EnableDebugLogging: checked,
      });
    }
  }

  function handleOnEnableInfoLoggingChange(checked: boolean) {
    if (appConfig) {
      setAppConfig({
        ...appConfig,
        EnableInfoLogging: checked,
      });
    }
  }

  function handleOnEnableWarningLoggingChange(checked: boolean) {
    if (appConfig) {
      setAppConfig({
        ...appConfig,
        EnableWarningLogging: checked,
      });
    }
  }

  function handleOnEnableErrorLoggingChange(checked: boolean) {
    if (appConfig) {
      setAppConfig({
        ...appConfig,
        EnableErrorLogging: checked,
      });
    }
  }
  function handleOnEnableFatalLoggingChange(checked: boolean) {
    if (appConfig) {
      setAppConfig({
        ...appConfig,
        EnableFatalLogging: checked,
      });
    }
  }

  function handleMaxLogCountChange(value: number) {
    if (appConfig) {
      setAppConfig({
        ...appConfig,
        MaxLogCount: value,
      });
    }
  }

  async function handleExport() {
    try {
      setIsLoading(true);
      const response: ApiResponse<any> = await exportAppConfig();
      if (response.Success) {
        Toast.success("Configuration exported successfully.");
      } else {
        Toast.error(response.ErrorMessage || "Failed to export configuration");
      }
    } finally {
      setIsLoading(false);
    }
  }

  async function handleImport() {
    try {
      setIsLoading(true);
      const path: string = await selectFileAbsolutePath([".json", "json"]);
      if (!path) return;
      const response: ApiResponse<any> = await importAppConfig(path);

      if (response.Success) {
        Toast.success(
          "Configuration imported successfully. Please restart the application to apply the changes."
        );

        const config: AppConfig = response.Data;
        setAppConfig(config);
        flash({});
      } else {
        Toast.error(response.ErrorMessage || "Failed to import configuration");
      }
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <div className="flex flex-col w-full h-full ">
      {/* enable loggings*/}
      <div className="w-full h-fit p-2 flex flex-row justify-between  border-b border-(--clr-surface-600) pb-2 ">
        <div className="flex flex-col  ">
          <span className="text-(--clr-text-primary) font-bold">
            Enable Logging to File
          </span>
          <span className="text-(--clr-text-secondary)">
            Enable saving logs to files. (Restart the application for changes to
            take effect.)
          </span>
        </div>
        <ModernSwitch
          checked={appConfig?.EnableLogging || false}
          onChange={(val: boolean) => handleOnEnableLoggingChange(val)}
        />
      </div>
      {/* log levels*/}
      <div className="w-full h-fit p-2 flex flex-row justify-between  border-b border-(--clr-surface-600) pb-2 ">
        <div className="flex flex-col  ">
          <span className="text-(--clr-text-primary) font-bold">
            Enable Logging to File
          </span>
          <span className="text-(--clr-text-secondary)">
            Enable saving logs to files. (Restart the application for changes to
            take effect.)
          </span>
        </div>
        <div className="flex flex-row gap-3">
          <ModernCheckButton
            className="border border-(--clr-surface-700)"
            checked={appConfig?.EnableTraceLogging || false}
            text="Trace"
            onChange={handleOnEnableTraceLoggingChange}
          />

          <ModernCheckButton
            className="border border-(--clr-surface-700)"
            checked={appConfig?.EnableDebugLogging || false}
            text="Debug"
            onChange={handleOnEnableDebugLoggingChange}
          />

          <ModernCheckButton
            className="border border-(--clr-surface-700)"
            checked={appConfig?.EnableInfoLogging || false}
            text="Info"
            onChange={handleOnEnableInfoLoggingChange}
          />
          <ModernCheckButton
            className="border border-(--clr-surface-700)"
            checked={appConfig?.EnableWarningLogging || false}
            text="Warning"
            onChange={handleOnEnableWarningLoggingChange}
          />
          <ModernCheckButton
            className="border border-(--clr-surface-700)"
            checked={appConfig?.EnableErrorLogging || false}
            text="Error"
            onChange={handleOnEnableErrorLoggingChange}
          />
          <ModernCheckButton
            className="border border-(--clr-surface-700)"
            checked={appConfig?.EnableFatalLogging || false}
            text="Fatal"
            onChange={handleOnEnableFatalLoggingChange}
          />
        </div>
      </div>
      <div className="w-full h-fit p-2 flex flex-row justify-between  border-b border-(--clr-surface-600) pb-2 ">
        <div className="flex flex-col  ">
          <span className="text-(--clr-text-primary) font-bold">
            Maximum Daily Log Files
          </span>
          <span className="text-(--clr-text-secondary)">
            Set the maximum number of daily log files to keep. (Restart the
            application for changes to take effect.)
          </span>
        </div>
        <div className="flex flex-row gap-3 w-24">
          <ModernNumberInput
            placeholder="20"
            value={
              appConfig?.MaxLogCount && appConfig?.MaxLogCount > 0
                ? appConfig.MaxLogCount + ""
                : ""
            }
            onChange={(val) => handleMaxLogCountChange(Number(val))}
            min={0}
            max={10000}
          />
        </div>
      </div>
      <div className="w-full h-fit p-2 flex flex-row justify-between  border-b border-(--clr-surface-600) pb-2 ">
        <div className="flex flex-col  ">
          <span className="text-(--clr-text-primary) font-bold">
            Maximum Daily Log Files
          </span>
          <span className="text-(--clr-text-secondary)">
            Set the maximum number of daily log files to keep. (Restart the
            application for changes to take effect.)
          </span>
        </div>
        <div className="flex flex-row gap-3 w-fit">
          <ModernButton
            text="Import"
            onMouseUp={handleImport}
            className="bg-(--clr-surface-100) text-(--clr-surface-900) hover:bg-(--clr-surface-200) border border-(--clr-surface-700) cursor-pointer "
          />
          <ModernButton
            text="Export"
            onMouseUp={handleExport}
            className="bg-(--clr-surface-100) text-(--clr-surface-900) hover:bg-(--clr-surface-200) border border-(--clr-surface-700) cursor-pointer "
          />
        </div>
      </div>
    </div>
  );
}

export { AdvancedSettingsPage };
