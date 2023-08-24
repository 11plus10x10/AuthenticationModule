import {Route} from "@angular/router";

export const ROUTES: Route[] = [
  {
    path: "signup", loadComponent: () =>
      import("../app/components/signup/signup.component")
        .then(mod => mod.SignupComponent)
  },
  {
    path: "signin", loadComponent: () =>
      import("../app/components/signin/signin.component")
        .then(mod => mod.SigninComponent)
  }
];
