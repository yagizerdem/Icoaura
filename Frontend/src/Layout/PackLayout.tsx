import { Outlet } from "react-router";
import { HorizontalSplitPane } from "../ui/HorizontalSplitPane";
import { PackConfigPanel } from "../Components/PackLayout/PackConfigPanel";

function PackLayout() {
  return (
    <div className="flex-1 h-full min-h-0  bg-(--clr-surface-800)">
      <HorizontalSplitPane
        leftPanelMinSize={200}
        leftPanelInitialSize={250}
        leftPanelMaxSize={350}
        leftChildren={<PackConfigPanel />}
        rightChildren={<Outlet />}
      />
    </div>
  );
}

export { PackLayout };
