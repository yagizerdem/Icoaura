import { ConfigListView } from "./ConfigListView";
import { PackConfigPanelTitle } from "./PackConfigPanelTitle";

function PackConfigPanel() {
  return (
    <div className="w-full h-full bg-(--clr-surface-700)  ">
      <PackConfigPanelTitle />
      <ConfigListView />
    </div>
  );
}

export { PackConfigPanel };
