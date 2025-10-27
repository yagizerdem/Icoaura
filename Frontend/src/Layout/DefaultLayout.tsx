import { Outlet } from "react-router";
import { TitleBar } from "../Components/DefaultLayout/TitleBar";
import { DefaultNavBar } from "../Components/DefaultLayout/DefaultNavBar";
import { useAppContext } from "../Providers/AppContext";
import { LoadPanel } from "../ui/LoadPanel";
import { Createpackpopup } from "../Components/Popup/CreatePackPopup";

function DefaultLayout() {
  const { isLoading, showCreatePackPopup } = useAppContext();

  return (
    <div className="w-screen h-screen bg-(--clr-surface-500) flex flex-col overflow-hidden">
      <div className="flash absolute w-full h-full top-0 left-0 inset-0 z-99999"></div>
      <TitleBar />
      <div className="flex min-h-0 flex-1 flex-col relative">
        {isLoading && <LoadPanel spinnerSize={50} spinnerColor="white" />}
        {showCreatePackPopup && <Createpackpopup />}

        <DefaultNavBar />
        <div className="flex-1 min-h-0">
          <Outlet />
        </div>
      </div>
    </div>
  );
}

export { DefaultLayout };
