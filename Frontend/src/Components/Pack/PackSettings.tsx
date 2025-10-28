import { useEffect, useState } from "react";
import { ModernSlider } from "../../ui/ModernSlider";
import { getSelectedPackConfig } from "../../util/getSelectedPackConfig";
import { useDebounce } from "../../hook/useDebounce";
import { WritePackConfig } from "../../service/packService";
import { Toast } from "../../util/toast";
import { usePackContext } from "../../Providers/PackContext";

function PackSettings() {
  const selectedPackConfig = getSelectedPackConfig();
  const [opacity, setOpacity] = useState(0);
  const [cornerRadius, setCornerRadius] = useState(0);
  const { packConfigs, setPackConfigs } = usePackContext();

  const debuncedOpacity = useDebounce(opacity, 300);
  const debuncedCornerRadius = useDebounce(cornerRadius, 300);
  const [hasUserEdit, setHasUserEdit] = useState(false);

  useEffect(() => {
    if (!selectedPackConfig) return;

    setOpacity(
      typeof selectedPackConfig.Opacity === "number"
        ? selectedPackConfig.Opacity * 100
        : 100
    );

    setCornerRadius(
      typeof selectedPackConfig.CornerRadius === "number"
        ? selectedPackConfig.CornerRadius * 100
        : 100
    );
  }, [selectedPackConfig]);

  useEffect(() => {
    if (!selectedPackConfig || !hasUserEdit) return;
    if (
      selectedPackConfig.CornerRadius != debuncedCornerRadius ||
      selectedPackConfig.Opacity != debuncedOpacity
    ) {
      // udpate pack config
      selectedPackConfig.CornerRadius = debuncedCornerRadius / 100;
      selectedPackConfig.Opacity = debuncedOpacity / 100;

      updatePackConfig();

      async function updatePackConfig() {
        if (selectedPackConfig) {
          const writeApiResponse = await WritePackConfig(selectedPackConfig);
          if (!writeApiResponse.Success) {
            Toast.error(
              writeApiResponse.ErrorMessage || "Failed to update pack settings."
            );
            return;
          }

          const updatedPackConfig = writeApiResponse.Data;
          const updatedPackConfigs = packConfigs.map((pc) =>
            pc.Uid === updatedPackConfig.Uid ? updatedPackConfig : pc
          );
          setPackConfigs(updatedPackConfigs);
        }
      }
    }
  }, [debuncedCornerRadius, debuncedOpacity]);

  return (
    <div className="w-full h-fit bg-(--clr-surface-800) p-3 rounded-md">
      <h1 className="text-xl text-(--clr-text-primary) font-medium">
        Pack Settings
      </h1>
      <hr className="my-2 border-(--clr-surface-500)" />
      <div className="flex flex-row items-center justify-between">
        <div className="flex flex-col">
          <span className="text-(--clr-text-primary) font-bold">
            Corner Radius
          </span>
          <span className="text-(--clr-text-secondary) font-sm text-sm">
            Changes the corner radius of icons in this pack
          </span>
        </div>
        <div className="flex flex-row items-center gap-2">
          <ModernSlider
            value={cornerRadius}
            min={0}
            max={50}
            onChange={(val) => {
              setHasUserEdit(true);
              setCornerRadius(val);
            }}
            showValue={false}
            className="w-96 "
          />
          <span className="w-15 font-bold text-(--clr-text-primary)">
            {cornerRadius === 100 ? "(100%)" : `(${cornerRadius.toFixed(0)}%)`}
          </span>
        </div>
      </div>

      <div className="flex flex-row items-center justify-between">
        <div className="flex flex-col">
          <span className="text-(--clr-text-primary) font-bold">Opacity</span>
          <span className="text-(--clr-text-secondary) font-sm text-sm">
            Changes the opacity of icons in this pack
          </span>
        </div>
        <div className="flex flex-row items-center gap-2">
          <ModernSlider
            value={opacity}
            min={10}
            max={100}
            onChange={(val) => {
              setHasUserEdit(true);
              setOpacity(val);
            }}
            showValue={false}
            className="w-96"
          />
          <span className="w-15 font-bold text-(--clr-text-primary)">
            {opacity === 100 ? "(100%)" : `(${opacity.toFixed(0)}%)`}
          </span>
        </div>
      </div>
    </div>
  );
}

export { PackSettings };
