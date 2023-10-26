import { Counter } from "./components/Counter";
import { DomikiPage } from "./components/DomikiPage";
import { Home } from "./components/Home";

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/counter',
    element: <Counter />
  },
  {
    path: '/domiki-page',
      element: <DomikiPage />
    }
];

export default AppRoutes;
