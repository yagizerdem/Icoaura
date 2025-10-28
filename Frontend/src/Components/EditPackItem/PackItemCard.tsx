import { useEffect, useState } from "react";
import type { PackItem } from "../../models/PackItem";
import { GetPackItemIconBase64 } from "../../service/packService";
import { getSelectedPackConfig } from "../../util/getSelectedPackConfig";
import { ChevronUp, CircleQuestionMark, Trash } from "lucide-react";

interface PackItemCardProps {
  packItems: PackItem[];
  selectedPackItem: PackItem;
  setPackItems: React.Dispatch<React.SetStateAction<PackItem[]>>;
  packItemIconMap: Record<string, string | null>;
  setPackItemIconMap: React.Dispatch<
    React.SetStateAction<Record<string, string | null>>
  >;
}

function PackItemCard({
  packItems,
  selectedPackItem,
  packItemIconMap,
  setPackItemIconMap,
  setPackItems,
}: PackItemCardProps) {
  const packConfig = getSelectedPackConfig();
  const iconBase64 = packItemIconMap[selectedPackItem.Uid];

  useEffect(() => {
    fetchIcon();
    async function fetchIcon() {
      if (!selectedPackItem || !selectedPackItem.Uid) return;
      if (!packConfig || !packConfig.Uid) return;

      const base64Icon: string =
        (await GetPackItemIconBase64(packConfig.Uid, selectedPackItem.Uid))
          .Data ?? "";

      if (base64Icon) {
        setPackItemIconMap((map) => {
          return { ...map, [selectedPackItem.Uid]: base64Icon };
        });
      }
    }
  }, [selectedPackItem]);

  function deletePackItem() {
    const updatedPackItems = packItems.filter(
      (item) => item.Uid !== selectedPackItem.Uid
    );
    setPackItems(updatedPackItems);
  }

  return (
    <div className="flex flex-row py-1 px-3 border-b border-gray-300 items-center">
      <div className="flex flex-row  w-full justify-between">
        <div className="flex flex-row items-center gap-4 ">
          {iconBase64 ? (
            <img src={iconBase64} alt="icon" className="w-12 h-12" />
          ) : (
            <div className="w-12 h-12 bg-(--clr-surface-950) rounded-sm">
              <CircleQuestionMark className="w-full h-full text-(--clr-text-primary)" />
            </div>
          )}
          <div className="text-(--clr-text-primary) font-bold ">
            {selectedPackItem.Name}
          </div>
        </div>
        <div className="flex flex-row gap-4 mx-5 items-center">
          <button
            onMouseUp={() => deletePackItem()}
            className="text-(--clr-text-primary) cursor-pointer hover:text-(--clr-text-secondary) transition-colors duration-200"
          >
            <Trash />
          </button>
          <button className="text-(--clr-text-primary) cursor-pointer hover:text-(--clr-text-secondary) transition-colors duration-200">
            <span>
              <ChevronUp />
            </span>
          </button>
        </div>
      </div>
    </div>
  );
}

export { PackItemCard };
