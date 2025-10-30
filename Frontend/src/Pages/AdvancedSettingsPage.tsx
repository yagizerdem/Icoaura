import { useAppContext } from "../Providers/AppContext";
import { ModernCheckButton } from "../ui/ModernCheckButton";
import { ModernSwitch } from "../ui/ModernSwitch";

function AdvancedSettingsPage() {
  const { appConfig, setAppConfig } = useAppContext();

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
            checked={appConfig?.EnableTraceLogging || false}
            text="Trace"
            onChange={handleOnEnableTraceLoggingChange}
          />
        </div>
      </div>
    </div>
  );
}

export { AdvancedSettingsPage };
