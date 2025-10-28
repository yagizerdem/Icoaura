import gsap from "gsap";
import { CurlyBraces, X } from "lucide-react";
import { useLayoutEffect, useRef, useState } from "react";
import { useAppContext } from "../../Providers/AppContext";
import { flash } from "../../util/cameraFlash";
import { ModernCheckBox } from "../../ui/ModernCheckbox";
import { ModernButton } from "../../ui/ModernButton";
import { DeletePack, getAllPackConfigs } from "../../service/packService";
import { getSelectedPackConfig } from "../../util/getSelectedPackConfig";
import { useNavigate } from "react-router";
import { usePackContext } from "../../Providers/PackContext";
import { Toast } from "../../util/toast";

function DeletePackPopup() {
  const cardRef = useRef<HTMLDivElement>(null);
  const { setShowDeletePackPopup, setIsLoading } = useAppContext();
  const { setPackConfigs } = usePackContext();
  const [deleteIcons, setDeleteIcons] = useState(false);
  const selectedPackConfig = getSelectedPackConfig();
  const navigate = useNavigate();

  useLayoutEffect(() => {
    if (cardRef.current) {
      gsap.fromTo(
        cardRef.current,
        { scale: 0.7, opacity: 0 },
        { scale: 1, opacity: 1, duration: 0.3, ease: "power2.out" }
      );
    }
  }, []);

  function closePopup(withFlash = false) {
    if (cardRef.current) {
      gsap.to(cardRef.current, {
        scale: 0.8,
        opacity: 0,
        duration: 0.2,
        ease: "power2.out",
        onComplete: () => {
          setShowDeletePackPopup(false);
          if (withFlash) {
            flash({});
          }
        },
      });
    }
  }

  async function deletePack() {
    try {
      setIsLoading(true);
      const apiResponse = await DeletePack(
        selectedPackConfig!.Uid,
        deleteIcons
      );

      if (!apiResponse.Success) {
        Toast.error(apiResponse.ErrorMessage || "Failed to delete package.");
        return;
      }

      const packConfigs = (await getAllPackConfigs()).Data || [];
      setPackConfigs(packConfigs);

      closePopup(true);
      navigate("/");
      Toast.success("Package deleted successfully.");
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <div className="w-full h-full bg-transparent absolute z-100 top-0 left-0 flex flex-col justify-center items-center gap-4">
      <div
        className="w-full h-full bg-black absolute z-101 opacity-90 left-0 right-0 inset-0 "
        onMouseUp={() => closePopup()}
      ></div>
      <div
        className="w-fit h-fit p-5 rounded-md bg-(--clr-surface-900) flex flex-col absolute z-102"
        ref={cardRef}
      >
        <div className="flex flex-row justify-between mb-2 gap-x-20 items-center">
          <div className="text-(--clr-text-primary) font-bold text-lg ">
            Are you sure you want to delete this package?
          </div>
          <button
            onMouseUp={() => closePopup()}
            className="cursor-pointer w-4 h-4 flex items-center justify-center text-(--clr-text-secondary) hover:text-(--clr-text-primary)"
          >
            <X />
          </button>
        </div>

        <div className="text-(--clr-text-secondary)  font-medium text-sm">
          This action cannot be reversed.
        </div>

        <div className="flex flex-row  gap-2 mt-6 ">
          <ModernCheckBox
            onMouseUp={(e) => e.stopPropagation()}
            onChange={(e: React.ChangeEvent<HTMLInputElement>) => {
              e.stopPropagation();
              setDeleteIcons(e.target.checked);
            }}
            checked={deleteIcons}
          />
          <div className="text-(--clr-text-secondary) font-md text-sm">
            deleting created icons
          </div>
        </div>

        <div className="mt-5 flex flex-row justify-end gap-3">
          <ModernButton
            text="Delete"
            type="danger"
            className="cursor-pointer"
            onMouseUp={() => deletePack()}
          />
          <ModernButton
            text="Cancel"
            type="ghost"
            className="cursor-pointer"
            onMouseUp={() => closePopup()}
          />
        </div>
      </div>
    </div>
  );
}

export { DeletePackPopup };
