import { useParams } from "react-router";
import { usePackContext } from "../Providers/PackContext";
import type { PackConfig } from "../models/PackConfig";

function getSelectedPackConfig(): PackConfig | null {
  const { packConfigs } = usePackContext();
  const params = useParams();
  const packId = params.packId as string;
  const selectedPack = packConfigs.find((c) => c.Uid == packId);

  return selectedPack ?? null;
}

export { getSelectedPackConfig };
