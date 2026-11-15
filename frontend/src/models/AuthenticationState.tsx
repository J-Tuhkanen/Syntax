import { ApplicationUser } from "./ApplicationUser";

export type AuthenticationState = {

  isSignedIn: boolean;
  User?: ApplicationUser;
};
