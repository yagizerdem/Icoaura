import { useEffect, useState } from "react";
import type { PackItem } from "../../models/PackItem";
import { getSelectedPackConfig } from "../../util/getSelectedPackConfig";
import {
  AppendPackItemFromPath,
  getAllPackConfigs,
  GetPackItemIconBase64,
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
import {
  getBase64FromPath,
  GetDirMetaData,
  GetLnkMetaData,
  GetUrlMetaData,
  selectDirectoryPath,
  selectFileAbsolutePath,
} from "../../service/fileService";
import type { ApiResponse } from "../../models/ApiResponse";
import type { LnkMetaData } from "../../models/LnkMetaData";
import type { UrlMetaData } from "../../models/UrlMetaData";

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

  async function handleAddLnkIcon() {
    try {
      if (!packConfig?.Uid) return;

      setIsLoading(true);
      const filePath = await selectFileAbsolutePath([".lnk", "lnk"]);
      if (!filePath) return;

      const lnkMetaDataResponse = await GetLnkMetaData(filePath);
      if (!lnkMetaDataResponse.Success || !lnkMetaDataResponse.Data) {
        Toast.error(
          lnkMetaDataResponse.ErrorMessage || "Failed to read lnk file."
        );
        return;
      }

      const metaData: LnkMetaData = lnkMetaDataResponse.Data;
      const base64 = (await getBase64FromPath(metaData.IconPath)).Data ?? "";

      // @ts-ignore
      const packItem: PackItem = {
        Uid: crypto.randomUUID(),
        TargetPath: filePath,
        TargetExePath: metaData.TargetPath,
        Description: metaData.Description,
        Name:
          filePath
            .split(/(\\|\/)/g)
            .pop()
            ?.split(".")[0] || "Unnamed",
      };

      setPackItems((prev) => [...prev, packItem]);
      if (base64) {
        // @ts-ignore
        console.log(base64);
        setPackItemIconMap((prev) => ({ ...prev, [packItem.Uid]: base64 }));
      }
      Toast.success("Lnk icon added successfully");
    } finally {
      setIsLoading(false);
    }
  }

  async function handleAddUrlIcon() {
    try {
      if (!packConfig?.Uid) return;

      setIsLoading(true);
      const filePath = await selectFileAbsolutePath([".url", "url"]);
      if (!filePath) return;

      const urlMetaDataResponse = await GetUrlMetaData(filePath);
      if (!urlMetaDataResponse.Success || !urlMetaDataResponse.Data) {
        Toast.error(
          urlMetaDataResponse.ErrorMessage || "Failed to read url file."
        );
        return;
      }

      const metaData: UrlMetaData = urlMetaDataResponse.Data;
      const base64 = (await getBase64FromPath(metaData.IconPath)).Data ?? "";

      // @ts-ignore
      const packItem: PackItem = {
        Uid: crypto.randomUUID(),
        TargetPath: filePath,
        TargetUrl: metaData.Url,
        Name:
          filePath
            .split(/(\\|\/)/g)
            .pop()
            ?.split(".")[0] || "Unnamed",
      };

      setPackItems((prev) => [...prev, packItem]);
      if (base64) {
        // @ts-ignore
        console.log(base64);
        setPackItemIconMap((prev) => ({ ...prev, [packItem.Uid]: base64 }));
      }

      Toast.success("Url icon added successfully");
    } finally {
      setIsLoading(false);
    }
  }

  async function handleAddDirIcon() {
    try {
      if (!packConfig?.Uid) return;

      setIsLoading(true);
      const dirPath = await selectDirectoryPath();

      if (!dirPath) return;

      const dirMetaDataResonse = await GetDirMetaData(dirPath);
      if (!dirMetaDataResonse.Success || !dirMetaDataResonse.Data) {
        Toast.error(
          dirMetaDataResonse.ErrorMessage || "Failed to read directory."
        );
        return;
      }

      const dirMetaData = dirMetaDataResonse.Data;
      const base64 = (await getBase64FromPath(dirMetaData.IconPath)).Data ?? "";
      // @ts-ignore
      const packItem: PackItem = {
        Uid: crypto.randomUUID(),
        TargetPath: dirPath,
        Name: dirPath.split(/(\\|\/)/g).pop() || "Unnamed",
      };

      setPackItems((prev) => [...prev, packItem]);
      if (base64) {
        // @ts-ignore
        console.log(base64);
        setPackItemIconMap((prev) => ({ ...prev, [packItem.Uid]: base64 }));
      }

      Toast.success("Directory icon added successfully");
      flash({});
    } finally {
      setIsLoading(false);
    }
  }

  async function handleAddEmptyFile() {
    // @ts-ignore
    const packItem: PackItem = {
      Uid: crypto.randomUUID(),
      Name: "Unknown",
    };
    setPackItems((prev) => [...prev, packItem]);
  }

  return (
    <div className="w-full h-full flex flex-col">
      <div className="flex-1 flex flex-row justify-between p-3">
        <div className="flex flex-row gap-3">
          <ModernButton
            type="ghost"
            className="bg-(--clr-surface-500) cursor-pointer"
            text="Add lnk icon"
            onMouseUp={handleAddLnkIcon}
          />
          <ModernButton
            type="ghost"
            className="bg-(--clr-surface-500) cursor-pointer"
            text="Add url icon"
            onMouseUp={handleAddUrlIcon}
          />
          <ModernButton
            type="ghost"
            className="bg-(--clr-surface-500) cursor-pointer"
            text="Add directory icon"
            onMouseUp={handleAddDirIcon}
          />
          <ModernButton
            text="Add empty file"
            className="cursor-pointer"
            onMouseUp={handleAddEmptyFile}
          />
        </div>
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
