import { Import, Plus, RefreshCcw } from "lucide-react";
import type { ApiResponse } from "../../models/ApiResponse";
import type { PackConfig } from "../../models/PackConfig";
import { getAllPackConfigs } from "../../service/packService";
import { usePackContext } from "../../Providers/PackContext";
import { useAppContext } from "../../Providers/AppContext";
import { ImportPack, selectFileAbsolutePath } from "../../service/fileService";
import { Toast } from "../../util/toast";

function PackConfigPanelTitle() {
  const { setPackConfigs } = usePackContext();
  const { setIsLoading, setShowCreatePackPopup } = useAppContext();

  async function RefreshPackConfig() {
    try {
      setIsLoading(true);
      const apiResponse: ApiResponse<PackConfig[]> = await getAllPackConfigs();
      if (apiResponse && apiResponse.Success) {
        setPackConfigs(apiResponse.Data);
      }
    } finally {
      setIsLoading(false);
    }
  }

  async function CreatePack() {
    setShowCreatePackPopup(true);
  }

  async function handleImportPack() {
    try {
      setIsLoading(true);
      const path = await selectFileAbsolutePath([".icr", "icr"]);

      console.log(path);
      const response: ApiResponse<void> = await ImportPack(path);
      if (response.Success) {
        await RefreshPackConfig();
        Toast.success("Pack imported successfully.");
      } else {
        Toast.error(response.ErrorMessage || "Failed to import pack.");
      }
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <div className=" flex flex-row justify-between items-center text-(--clr-text-primary) w-full h-8 bg-(--clr-surface-700)">
      <div className="flex flex-row  gap-2 px-3 py-2 h-full">
        <button
          onMouseUp={() => CreatePack()}
          className="w-6 h-6 p-1 cursor-pointer bg-(--clr-surface-600) flex justify-center items-center rounded hover:bg-(--clr-surface-500)"
        >
          <Plus />
        </button>
        <button
          onMouseUp={() => RefreshPackConfig()}
          className="w-6 h-6 p-1 cursor-pointer bg-(--clr-surface-600) flex justify-center items-center rounded hover:bg-(--clr-surface-500)"
        >
          <RefreshCcw />
        </button>
      </div>
      <div
        className="px-3  flex-row  gap-2 py-2 h-full "
        onMouseUp={() => handleImportPack()}
      >
        <button className="w-6 h-6 p-1 cursor-pointer bg-(--clr-surface-600) flex justify-center items-center rounded hover:bg-(--clr-surface-500)">
          <Import />
        </button>
      </div>
    </div>
  );
}

export { PackConfigPanelTitle };
