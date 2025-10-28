import { useEffect, useState } from "react";
import type { PackItem } from "../../models/PackItem";
import { getSelectedPackConfig } from "../../util/getSelectedPackConfig";
import {
  getAllPackConfigs,
  GetPackItems,
  WritePackItems,
} from "../../service/packService";
import { PackItemCard } from "./PackItemCard";
import { ModernButton } from "../../ui/ModernButton";
import { usePackContext } from "../../Providers/PackContext";
import { useAppContext } from "../../Providers/AppContext";
import { Toast } from "../../util/toast";
import type { PackConfig } from "../../models/PackConfig";
import { flash } from "../../util/cameraFlash";

function EditPackItemPanel() {
  const { setEditPackItemMode, setPackConfigs } = usePackContext();
  const { setIsLoading } = useAppContext();

  const packConfig = getSelectedPackConfig();
  const [packItems, setPackItems] = useState<PackItem[]>([]);
  const [packItemIconMap, setPackItemIconMap] = useState<
    Record<string, string>
  >({});

  useEffect(() => {
    fetchPackItems();
    async function fetchPackItems() {
      if (!packConfig || !packConfig.Uid) return;

      const response = await GetPackItems(packConfig.Uid);
      if (response.Success && response.Data) {
        const packItems = response.Data;
        packItems.sort((a, b) => {
          const aPriority = a.TargetPath.endsWith(".lnk")
            ? 2
            : a.TargetPath.endsWith(".url")
            ? 1
            : 0;

          const bPriority = b.TargetPath.endsWith(".lnk")
            ? 2
            : b.TargetPath.endsWith(".url")
            ? 1
            : 0;

          return bPriority - aPriority;
        });
        setPackItems(packItems);
      }
    }
  }, [packConfig]);

  async function updatePackItems() {
    try {
      if (!packConfig || !packConfig.Uid) return;

      setIsLoading(true);
      const apiResponse = await WritePackItems(
        packConfig.Uid,
        packItems,
        packItemIconMap
      );

      if (!apiResponse.Success) {
        Toast.error(apiResponse.ErrorMessage || "Failed to update pack items.");
        return;
      }

      const packConfigs: PackConfig[] = (await getAllPackConfigs()).Data || [];
      setPackConfigs(packConfigs);

      flash({});
      setEditPackItemMode(false);
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <div className="w-full h-full flex flex-col">
      <div className="flex-1 flex flex-row justify-between p-3">
        <div className="flex flex-row "></div>
        <div className="flex flex-row mx-3 gap-3">
          <ModernButton
            text="Update"
            type="success"
            className="cursor-pointer"
            onMouseUp={() => updatePackItems()}
          />

          <ModernButton
            className="cursor-pointer"
            text="Cancel"
            type="danger"
            onMouseUp={() => {
              setEditPackItemMode(false);
            }}
          />
        </div>
      </div>
      <div className="flex flex-col w-full h-full  overflow-y-auto">
        {packItems.map((item) => (
          <PackItemCard
            key={item.Uid}
            packItems={packItems}
            selectedPackItem={item}
            setPackItems={setPackItems}
            packItemIconMap={packItemIconMap}
            setPackItemIconMap={setPackItemIconMap}
          />
        ))}
      </div>
    </div>
  );
}

export { EditPackItemPanel };
