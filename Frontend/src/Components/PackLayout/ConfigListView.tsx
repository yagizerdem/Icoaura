import { useEffect } from "react";
import { usePackContext } from "../../Providers/PackContext";
import type { ApiResponse } from "../../models/ApiResponse";
import type { PackConfig } from "../../models/PackConfig";
import { getAllPackConfigs } from "../../service/packService";
import { PackConfigCard } from "./PackConfigCard";

function ConfigListView() {
  const { packConfigs, setPackConfigs } = usePackContext();

  useEffect(() => {
    fetch();
    async function fetch() {
      const apiResponse: ApiResponse<PackConfig[]> = await getAllPackConfigs();
      if (apiResponse && apiResponse?.Success) {
        apiResponse.Data.forEach((c) => {
          setPackConfigs((prev) => [...prev, c]);
          setPackConfigs((prev) => [...prev, c]);
          setPackConfigs((prev) => [...prev, c]);
          setPackConfigs((prev) => [...prev, c]);
        });
      }
    }
  }, []);

  console.log(packConfigs);

  return (
    <div className="w-full h-full  overflow-y-auto">
      {packConfigs.map((config, i) => (
        <PackConfigCard key={i} packConfig={config} />
      ))}
    </div>
  );
}

export { ConfigListView };
