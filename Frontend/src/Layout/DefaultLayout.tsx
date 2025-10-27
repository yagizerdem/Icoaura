import { Outlet } from "react-router";
import { TitleBar } from "../Components/DefaultLayout/TitleBar";
import { DefaultNavBar } from "../Components/DefaultLayout/DefaultNavBar";
import { useAppContext } from "../Providers/AppContext";
import { LoadPanel } from "../ui/LoadPanel";

function DefaultLayout() {
  const { isLoading } = useAppContext();

  return (
    <div className="w-screen h-screen bg-(--clr-surface-500) flex flex-col ">
      <TitleBar />
      <div className="flex flex-1 flex-col relative">
        {isLoading && <LoadPanel spinnerSize={50} spinnerColor="white" />}

        <DefaultNavBar />
        <div className="flex flex-1 min-h-0">
          <Outlet />
        </div>
      </div>
    </div>
  );
}

export { DefaultLayout };
