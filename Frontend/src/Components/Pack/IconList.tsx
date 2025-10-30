import { useEffect, useState } from "react";
import { getSelectedPackConfig } from "../../util/getSelectedPackConfig";
import type { ApiResponse } from "../../models/ApiResponse";
import type { PackItem } from "../../models/PackItem";
import { GetPackItems } from "../../service/packService";
import { IconDisplay } from "./IconDisplay";
import { useL10NContext } from "../../Providers/L10NContext";

function IconList() {
  const packConfig = getSelectedPackConfig();
  const [packItems, setPackItems] = useState<PackItem[]>([]);

  const { getLocalizedString } = useL10NContext();

  useEffect(() => {
    fetchPackItems();
    async function fetchPackItems() {
      if (!packConfig || !packConfig.Uid) return;

      const apiResponse: ApiResponse<PackItem[]> = await GetPackItems(
        packConfig.Uid
      );

      if (apiResponse.Success && apiResponse.Data) {
        setPackItems(apiResponse.Data || []);
      }
    }
  }, [packConfig]);

  return (
    <div className="w-full h-fit bg-(--clr-surface-800) p-3 rounded-md">
      <h1 className="text-xl text-(--clr-text-primary) font-medium">
        {getLocalizedString("Package.Icons.Title")}
      </h1>
      <hr className="my-2 border-(--clr-surface-500)" />
      <div className="flex flex-wrap gap-2 items-center justify-center align-middle">
        {packItems.map((item) => {
          return <IconDisplay key={item.Uid} packItem={item} />;
        })}
      </div>
    </div>
  );
}

export { IconList };
