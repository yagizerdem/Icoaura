import { Outlet } from "react-router";
import { TitleBar } from "../Components/DefaultLayout/TitleBar";

function DefaultLayout() {
  return (
    <div className="w-screen h-screen bg-(--clr-surface-500) ">
      <TitleBar />
      <Outlet />
    </div>
  );
}

export { DefaultLayout };
