import { PackInfo } from "../Components/Pack/PackInfo";

function PackPage() {
  return (
    <div className="w-full h-full overflow-y-auto p-5 bg-(--clr-surface-900)">
      <PackInfo />
    </div>
  );
}

export { PackPage };
