import { useAppContext } from "../Providers/AppContext";
import { useL10NContext } from "../Providers/L10NContext";
import { ModernSlider } from "../ui/ModernSlider";
import { ModernSwitch } from "../ui/ModernSwitch";

function IconPackSettingsPage() {
  const { appConfig, setAppConfig } = useAppContext();
  const { getLocalizedString } = useL10NContext();

  function handleToggleMatchLnkByTargetExe(checked: boolean) {
    if (appConfig) {
      setAppConfig({
        ...appConfig,
        MatchLnkByTargetExe: checked,
      });
    }
  }

  function handleToggleMatchUrlByTargetUrl(checked: boolean) {
    if (appConfig) {
      setAppConfig({
        ...appConfig,
        MatchUrlByTargetUrl: checked,
      });
    }
  }

  function handleChangeDescriptionOfMatchedLnkFiles(checked: boolean) {
    if (appConfig) {
      setAppConfig({
        ...appConfig,
        ChangeDescriptionOfMatchedLnkFiles: checked,
      });
    }
  }

  function handleForceExplorerRefreshAfterIcoChange(checked: boolean) {
    if (appConfig) {
      setAppConfig({
        ...appConfig,
        ForceExplorerRefreshAfterIcoChange: checked,
      });
    }
  }

  function handleChangeRecursiveScanningDepth(value: number) {
    if (appConfig) {
      setAppConfig({
        ...appConfig,
        RecursiveScanningDepth: value,
      });
    }
  }

  return (
    <div className="flex flex-col w-full h-full ">
      {/* match lnk by target exe */}
      <div className="w-full h-fit p-2 flex flex-row justify-between  border-b border-(--clr-surface-600) pb-2 ">
        <div className="flex flex-col  ">
          <span className="text-(--clr-text-primary) font-bold">
            {getLocalizedString(
              "Settings.IconPackSettings.MatchLnkByTargetExe.Title"
            )}
          </span>
          <span className="text-(--clr-text-secondary)">
            {getLocalizedString(
              "Settings.IconPackSettings.MatchLnkByTargetExe.Description"
            )}
          </span>
        </div>
        <ModernSwitch
          checked={appConfig?.MatchLnkByTargetExe ?? false}
          onChange={handleToggleMatchLnkByTargetExe}
        />
      </div>
      {/* match url by target url */}
      <div className="w-full h-fit p-2 flex flex-row justify-between  border-b border-(--clr-surface-600) pb-2 ">
        <div className="flex flex-col  ">
          <span className="text-(--clr-text-primary) font-bold">
            {getLocalizedString(
              "Settings.IconPackSettings.MatchUrlByTargetUrl.Title"
            )}
          </span>
          <span className="text-(--clr-text-secondary)">
            {getLocalizedString(
              "Settings.IconPackSettings.MatchUrlByTargetUrl.Description"
            )}
          </span>
        </div>
        <ModernSwitch
          checked={appConfig?.MatchUrlByTargetUrl ?? false}
          onChange={handleToggleMatchUrlByTargetUrl}
        />
      </div>

      {/* change description of matched windows shortcut files */}
      <div className="w-full h-fit p-2 flex flex-row justify-between  border-b border-(--clr-surface-600) pb-2 ">
        <div className="flex flex-col  ">
          <span className="text-(--clr-text-primary) font-bold">
            {getLocalizedString(
              "Settings.IconPackSettings.ChangeDescriptionOfMatchedLnkFiles.Title"
            )}
          </span>
          <span className="text-(--clr-text-secondary)">
            {getLocalizedString(
              "Settings.IconPackSettings.ChangeDescriptionOfMatchedLnkFiles.Description"
            )}
          </span>
        </div>
        <ModernSwitch
          checked={appConfig?.ChangeDescriptionOfMatchedLnkFiles ?? false}
          onChange={handleChangeDescriptionOfMatchedLnkFiles}
        />
      </div>

      {/* ForceExplorerRefreshAfterIcoChange */}
      <div className="w-full h-fit p-2 flex flex-row justify-between  border-b border-(--clr-surface-600) pb-2 ">
        <div className="flex flex-col  ">
          <span className="text-(--clr-text-primary) font-bold">
            {getLocalizedString(
              "Settings.IconPackSettings.ForceExplorerRefreshAfterIcoChange.Title"
            )}
          </span>
          <span className="text-(--clr-text-secondary)">
            {getLocalizedString(
              "Settings.IconPackSettings.ForceExplorerRefreshAfterIcoChange.Description"
            )}
          </span>
        </div>
        <ModernSwitch
          className="min-w-12"
          checked={appConfig?.ForceExplorerRefreshAfterIcoChange ?? false}
          onChange={handleForceExplorerRefreshAfterIcoChange}
        />
      </div>

      {/* recursive scanning depth */}
      <div className="w-full h-fit p-2 flex flex-row justify-between items-center  border-b border-(--clr-surface-600) pb-2 ">
        <div className="flex flex-col  ">
          <span className="text-(--clr-text-primary) font-bold">
            {getLocalizedString(
              "Settings.IconPackSettings.RecursiveScanningDepth.Title"
            )}
          </span>
          <span className="text-(--clr-text-secondary)">
            {getLocalizedString(
              "Settings.IconPackSettings.RecursiveScanningDepth.Description"
            )}
          </span>
        </div>
        <div className="flex flex-row gap-3 items-center">
          <ModernSlider
            showValue={false}
            className="w-64"
            value={appConfig?.RecursiveScanningDepth ?? 0}
            onChange={handleChangeRecursiveScanningDepth}
            min={0}
            max={5}
            step={1}
          />
          <div className="text-(--clr-text-secondary) w-fit mr-4  font-bold">
            {appConfig?.RecursiveScanningDepth ?? 0}
          </div>
        </div>
      </div>
    </div>
  );
}

export { IconPackSettingsPage };
