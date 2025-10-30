import type { ApiResponse } from "../models/ApiResponse";
import type { AppConfig } from "../models/AppConfig";
import { useAppContext } from "../Providers/AppContext";
import { useL10NContext } from "../Providers/L10NContext";
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
  const { getLocalizedString } = useL10NContext();

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
        Toast.success(
          getLocalizedString(
            "Settings.AdvancedSettings.ImportExport.ExportSuccess"
          )
        );
      } else {
        Toast.error(
          response.ErrorMessage ||
            getLocalizedString(
              "Settings.AdvancedSettings.ImportExport.ExportError"
            )
        );
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
          getLocalizedString(
            "Settings.AdvancedSettings.ImportExport.ImportSuccess"
          )
        );

        const config: AppConfig = response.Data;
        setAppConfig(config);
        flash({});
      } else {
        Toast.error(
          response.ErrorMessage ||
            getLocalizedString(
              "Settings.AdvancedSettings.ImportExport.ImportError"
            )
        );
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
            {getLocalizedString(
              "Settings.AdvancedSettings.EnableLogging.Title"
            )}
          </span>
          <span className="text-(--clr-text-secondary)">
            {getLocalizedString(
              "Settings.AdvancedSettings.EnableLogging.Description"
            )}
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
            {getLocalizedString("Settings.AdvancedSettings.LogLevels.Title")}
          </span>
          <span className="text-(--clr-text-secondary)">
            {getLocalizedString(
              "Settings.AdvancedSettings.LogLevels.Description"
            )}
          </span>
        </div>
        <div className="flex flex-row gap-3">
          <ModernCheckButton
            className="border border-(--clr-surface-700)"
            checked={appConfig?.EnableTraceLogging || false}
            text={getLocalizedString(
              "Settings.AdvancedSettings.LogLevels.Trace"
            )}
            onChange={handleOnEnableTraceLoggingChange}
          />

          <ModernCheckButton
            className="border border-(--clr-surface-700)"
            checked={appConfig?.EnableDebugLogging || false}
            text={getLocalizedString(
              "Settings.AdvancedSettings.LogLevels.Debug"
            )}
            onChange={handleOnEnableDebugLoggingChange}
          />

          <ModernCheckButton
            className="border border-(--clr-surface-700)"
            checked={appConfig?.EnableInfoLogging || false}
            text={getLocalizedString(
              "Settings.AdvancedSettings.LogLevels.Info"
            )}
            onChange={handleOnEnableInfoLoggingChange}
          />
          <ModernCheckButton
            className="border border-(--clr-surface-700)"
            checked={appConfig?.EnableWarningLogging || false}
            text={getLocalizedString(
              "Settings.AdvancedSettings.LogLevels.Warning"
            )}
            onChange={handleOnEnableWarningLoggingChange}
          />
          <ModernCheckButton
            className="border border-(--clr-surface-700)"
            checked={appConfig?.EnableErrorLogging || false}
            text={getLocalizedString(
              "Settings.AdvancedSettings.LogLevels.Error"
            )}
            onChange={handleOnEnableErrorLoggingChange}
          />
          <ModernCheckButton
            className="border border-(--clr-surface-700)"
            checked={appConfig?.EnableFatalLogging || false}
            text={getLocalizedString(
              "Settings.AdvancedSettings.LogLevels.Fatal"
            )}
            onChange={handleOnEnableFatalLoggingChange}
          />
        </div>
      </div>
      <div className="w-full h-fit p-2 flex flex-row justify-between  border-b border-(--clr-surface-600) pb-2 ">
        <div className="flex flex-col  ">
          <span className="text-(--clr-text-primary) font-bold">
            {getLocalizedString("Settings.AdvancedSettings.MaxLogCount.Title")}
          </span>
          <span className="text-(--clr-text-secondary)">
            {getLocalizedString(
              "Settings.AdvancedSettings.MaxLogCount.Description"
            )}
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
            {getLocalizedString("Settings.AdvancedSettings.ImportExport.Title")}
          </span>
          <span className="text-(--clr-text-secondary)">
            {getLocalizedString(
              "Settings.AdvancedSettings.ImportExport.Description"
            )}
          </span>
        </div>
        <div className="flex flex-row gap-3 w-fit">
          <ModernButton
            text={getLocalizedString(
              "Settings.AdvancedSettings.ImportExport.Import"
            )}
            onMouseUp={handleImport}
            className="bg-(--clr-surface-100) text-(--clr-surface-900) hover:bg-(--clr-surface-200) border border-(--clr-surface-700) cursor-pointer "
          />
          <ModernButton
            text={getLocalizedString(
              "Settings.AdvancedSettings.ImportExport.Export"
            )}
            onMouseUp={handleExport}
            className="bg-(--clr-surface-100) text-(--clr-surface-900) hover:bg-(--clr-surface-200) border border-(--clr-surface-700) cursor-pointer "
          />
        </div>
      </div>
    </div>
  );
}

export { AdvancedSettingsPage };
