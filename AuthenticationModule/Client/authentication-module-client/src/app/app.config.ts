import { ApplicationConfig } from '@angular/core';
import { provideAnimations } from '@angular/platform-browser/animations';
import {provideRouter} from "@angular/router";
import {ROUTES} from "../routes/routes";
import {provideHttpClient} from "@angular/common/http";

export const appConfig: ApplicationConfig = {
  providers: [provideAnimations(), provideRouter(ROUTES), provideHttpClient()]
};
