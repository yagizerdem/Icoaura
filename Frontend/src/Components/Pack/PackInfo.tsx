import { CircleQuestionMark, PenBox, Trash } from "lucide-react";
import { getSelectedPackConfig } from "../../util/getSelectedPackConfig";
import { usePackContext } from "../../Providers/PackContext";
import { useAppContext } from "../../Providers/AppContext";

function PackInfo() {
  const packConfig = getSelectedPackConfig();
  const { setEditPackConfigMode } = usePackContext();
  const { setShowDeletePackPopup } = useAppContext();

  function editPackConfig() {
    setEditPackConfigMode(true);
  }

  function deletePack() {
    setShowDeletePackPopup(true);
  }

  return (
    <div className="w-full h-fit bg-(--clr-surface-800) p-3 rounded-md">
      <h1 className="text-xl text-(--clr-text-primary) font-medium">
        Pack Information
      </h1>
      <hr className="my-2 border-(--clr-surface-500)" />
      <div className="flex flex-row justify-between items-center">
        <div className="flex flex-row gap-5 ">
          {packConfig?.CoverPngBase64 && (
            <img
              src={packConfig?.CoverPngBase64}
              alt={packConfig?.PackName}
              className="w-12 h-12 object-cover rounded-md"
            />
          )}
          {!packConfig?.CoverPngBase64 && (
            <div className="w-12 h-12 relative bg-(--clr-surface-900) rounded-sm">
              <CircleQuestionMark className="w-full h-full text-(--clr-text-primary)" />
            </div>
          )}

          <div className="flex flex-col ">
            <span className="text-left text-(--clr-text-primary) font-bold">
              Name
            </span>
            <span className="text-center  text-(--clr-text-secondary) font-medium">
              {packConfig?.PackName && packConfig?.PackName.length > 20
                ? `${packConfig?.PackName.slice(0, 20)}...`
                : packConfig?.PackName}
            </span>
          </div>
          <div className="flex flex-col ">
            <span className="text-left text-(--clr-text-primary) font-bold">
              Version
            </span>
            <span className="text-center  text-(--clr-text-secondary) font-medium">
              {packConfig?.Version.startsWith("v")
                ? packConfig?.Version
                : `v${packConfig?.Version}`}
            </span>
          </div>
          <div className="flex flex-col ">
            <span className="text-left text-(--clr-text-primary) font-bold">
              Author
            </span>
            <span className="text-center  text-(--clr-text-secondary) font-medium">
              {packConfig?.Author && packConfig.Author.length > 20
                ? `${packConfig?.Author.slice(0, 20)}...`
                : packConfig?.Author}
            </span>
          </div>
          <div className="flex flex-col ">
            <span className="text-left text-(--clr-text-primary) font-bold">
              License
            </span>
            <span className="text-center  text-(--clr-text-secondary) font-medium">
              {packConfig?.License && packConfig.License.length > 20
                ? `${packConfig?.License.slice(0, 20)}...`
                : packConfig?.License}
            </span>
          </div>
        </div>
        <div className="flex flex-row gap-3">
          <button
            onMouseUp={() => editPackConfig()}
            className="w-8 h-8 p-1 cursor-pointer text-(--clr-text-primary) flex justify-center items-center rounded"
          >
            <PenBox />
          </button>

          <button
            onMouseUp={() => deletePack()}
            className="w-8 h-8 p-1 cursor-pointer  text-(--clr-text-primary) flex justify-center items-center rounded "
          >
            <Trash />
          </button>
        </div>
      </div>
      <div>
        {packConfig?.Description && packConfig.Description.length > 50 ? (
          <p className="mt-4 text-(--clr-text-secondary)">
            {`${packConfig?.Description.slice(0, 400)}...`}
          </p>
        ) : (
          <p className="mt-4 text-(--clr-text-secondary)">
            {packConfig?.Description}
          </p>
        )}
      </div>
    </div>
  );
}

export { PackInfo };
