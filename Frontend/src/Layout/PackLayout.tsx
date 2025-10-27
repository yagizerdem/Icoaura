import { Outlet } from "react-router";
import { HorizontalSplitPane } from "../ui/HorizontalSplitPane";
import { PackItemsPanel } from "../Components/PackLayout/PackItemsPanel";

function PackLayout() {
  return (
    <div className="flex flex-1 flex-col bg-(--clr-surface-600)">
      <HorizontalSplitPane
        leftPanelMinSize={200}
        leftPanelInitialSize={250}
        leftPanelMaxSize={350}
        leftChildren={<PackItemsPanel />}
        rightChildren={<Outlet />}
      />
    </div>
  );
}

export { PackLayout };
