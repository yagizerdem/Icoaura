import { EditPackItemPanel } from "../Components/EditPackItem/EditPackItemPanel";
import { EditPackConfig } from "../Components/Pack/EditPackConfig";
import { IconList } from "../Components/Pack/IconList";
import { PackInfo } from "../Components/Pack/PackInfo";
import { PackOperation } from "../Components/Pack/PackOperation";
import { PackSettings } from "../Components/Pack/PackSettings";
import { usePackContext } from "../Providers/PackContext";

function PackPage() {
  const { editPackConfigMode, editPackItemMode } = usePackContext();

  return (
    <div className="w-full h-full bg-(--clr-surface-900) overflow-y-auto">
      {editPackItemMode ? (
        <EditPackItemPanel />
      ) : (
        <div className="p-5">
          {editPackConfigMode ? <EditPackConfig /> : <PackInfo />}
          <br />
          <PackOperation />
          <br />
          <PackSettings />
          <br />
          <IconList />
        </div>
      )}
    </div>
  );
}

export { PackPage };
