import { useLocation, useNavigate } from "react-router";
import { ModernButton } from "../../Ui/ModernButton";
import { cn } from "../../util/twUtil";

function DefaultNavBar() {
  const location = useLocation();
  const navigate = useNavigate();
  console.log(location.pathname);

  function navigateToPack() {
    navigate("/pack");
  }

  function navigateToSettings() {
    navigate("/settings");
  }

  return (
    <div className="w-full h-12 bg-(--clr-surface-700)  flex items-center flex-row px-3 gap-3">
      <ModernButton
        text="Pack"
        type="ghost"
        className={cn(
          "cursor-pointer transition-colors duration-300 hover:bg-(--clr-surface-700) bg-(--clr-surface-800) border-none",
          location.pathname.startsWith("/pack") &&
            "font-bold bg-(--clr-surface-950) "
        )}
        onMouseUp={navigateToPack}
      />
      <ModernButton
        text="Settings"
        type="ghost"
        className={cn(
          "cursor-pointer transition-colors duration-300  hover:bg-(--clr-surface-700) bg-(--clr-surface-800) border-none",
          location.pathname.startsWith("/settings") &&
            "font-bold bg-(--clr-surface-950) "
        )}
        onMouseUp={navigateToSettings}
      />
    </div>
  );
}

export { DefaultNavBar };
