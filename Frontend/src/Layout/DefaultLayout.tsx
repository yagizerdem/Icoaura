import { Outlet } from "react-router";
import { TitleBar } from "../Components/DefaultLayout/TitleBar";
import { DefaultNavBar } from "../Components/DefaultLayout/DefaultNavBar";

function DefaultLayout() {
  return (
    <div className="w-screen h-screen bg-(--clr-surface-500) ">
      <TitleBar />
      <DefaultNavBar />
      <Outlet />
    </div>
  );
}

export { DefaultLayout };
