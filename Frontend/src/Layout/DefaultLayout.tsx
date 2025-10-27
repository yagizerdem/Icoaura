import { Outlet } from "react-router";
import { TitleBar } from "../Components/DefaultLayout/TitleBar";
import { DefaultNavBar } from "../Components/DefaultLayout/DefaultNavBar";

function DefaultLayout() {
  return (
    <div className="w-screen h-screen bg-(--clr-surface-500) flex flex-col ">
      <TitleBar />
      <DefaultNavBar />
      <div className="flex flex-1 min-h-0">
        <Outlet />
      </div>
    </div>
  );
}

export { DefaultLayout };
