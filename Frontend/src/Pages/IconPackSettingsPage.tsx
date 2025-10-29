import { useAppContext } from "../Providers/AppContext";
import { ModernSlider } from "../ui/ModernSlider";
import { ModernSwitch } from "../ui/ModernSwitch";

function IconPackSettingsPage() {
  const { appConfig, setAppConfig } = useAppContext();

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
            Match Windows Shortcuts (.lnk) by Their Targets
          </span>
          <span className="text-(--clr-text-secondary)">
            If matching by file path fails, match shortcuts by their target
            paths.
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
            Match URL Files by Their URL
          </span>
          <span className="text-(--clr-text-secondary)">
            If matching by file path fails, match files by their URLs.
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
            Change Description of Matched Windows Shortcut (.lnk) Files
          </span>
          <span className="text-(--clr-text-secondary)">
            Change the description of matched shortcut files.
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
            Force Explorer Refresh After Icon Change
          </span>
          <span className="text-(--clr-text-secondary)">
            This option forces Windows Explorer to refresh aggressively after
            changing an icon. It ensures icons update immediately, but may cause
            temporary lag or high CPU usage on low-end systems.
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
            Recursive Scanning Depth
          </span>
          <span className="text-(--clr-text-secondary)">
            The maximum depth of windows special subdirectories to scan for icon
            files to match target executables. Depth only matters when match lnk
            by target option is enabled.
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
