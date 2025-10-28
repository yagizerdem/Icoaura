import { EditPackConfig } from "../Components/Pack/EditPackConfig";
import { PackInfo } from "../Components/Pack/PackInfo";
import { PackOperation } from "../Components/Pack/PackOperation";
import { PackSettings } from "../Components/Pack/PackSettings";
import { usePackContext } from "../Providers/PackContext";

function PackPage() {
  const { editPackConfigMode } = usePackContext();

  return (
    <div className="w-full h-full overflow-y-auto p-5 bg-(--clr-surface-900)">
      {editPackConfigMode ? <EditPackConfig /> : <PackInfo />}
      <br />
      <PackOperation />
      <br />
      <PackSettings />
    </div>
  );
}

export { PackPage };
