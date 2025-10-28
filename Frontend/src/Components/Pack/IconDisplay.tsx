import { useEffect, useState } from "react";
import type { PackItem } from "../../models/PackItem";
import { getSelectedPackConfig } from "../../util/getSelectedPackConfig";
import type { ApiResponse } from "../../models/ApiResponse";
import { GetPackItemIconBase64 } from "../../service/packService";
import { ShinyBox } from "../../ui/ShinyBox";
import { cn } from "../../util/twUtil";

interface IconDisplayProps {
  packItem: PackItem;
}

function IconDisplay({ packItem }: IconDisplayProps) {
  const packConfigUid = getSelectedPackConfig()?.Uid;
  const packItemUid = packItem.Uid;
  const [icoBase64, setIcoBase64] = useState<string | null>(null);
  const packConfig = getSelectedPackConfig();

  useEffect(() => {
    fetchIcon();
    async function fetchIcon() {
      if (!packConfigUid || !packItemUid) return;

      const apiResponse: ApiResponse<string | null> =
        await GetPackItemIconBase64(packConfigUid, packItemUid);

      if (apiResponse.Success && apiResponse.Data) {
        setIcoBase64(apiResponse.Data);
      }
    }
  }, [packConfigUid, packItemUid]);

  if (icoBase64) {
    return (
      <img
        src={icoBase64}
        alt={packItem.Name}
        className={cn(
          "w-12 h-12 border border-(--clr-surface-500)",
          packConfig?.CornerRadius &&
            packConfig.CornerRadius > 0 &&
            "rounded-none"
        )}
        style={{
          borderRadius: packConfig?.CornerRadius
            ? `${packConfig.CornerRadius * 100}%`
            : "0%",
          opacity: packConfig?.Opacity ?? 1,
        }}
      />
    );
  }

  if (
    !icoBase64 &&
    (packItem.TargetPath.endsWith(".url") ||
      packItem.TargetPath.endsWith(".lnk"))
  ) {
    return <ShinyBox />;
  }
}

export { IconDisplay };
