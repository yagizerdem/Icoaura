import { useState } from "react";
import { ModernSlider } from "../../ui/ModernSlider";

function PackSettings() {
  const [opacity, setOpacity] = useState(100);

  return (
    <div className="w-full h-fit bg-(--clr-surface-800) p-3 rounded-md">
      <h1 className="text-xl text-(--clr-text-primary) font-medium">
        Pack Settings
      </h1>
      <hr className="my-2 border-(--clr-surface-500)" />
      <ModernSlider
        label="Opacity"
        value={opacity}
        min={0}
        max={100}
        onChange={setOpacity}
        showValue={false}
      />
    </div>
  );
}

export { PackSettings };
