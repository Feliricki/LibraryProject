import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { importProvidersFrom } from '@angular/core';
import { AppComponent } from './app/app.component';
import { withInterceptorsFromDi, provideHttpClient, withInterceptors } from '@angular/common/http';
import { BrowserModule, bootstrapApplication } from '@angular/platform-browser';
import { AppRoutingModule } from "./app/app-routing.module";
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import {authInterceptor} from "./app/auth/auth.interceptor";


bootstrapApplication(AppComponent, {
    providers: [
        importProvidersFrom(BrowserModule, AppRoutingModule),
        provideHttpClient(withInterceptors([authInterceptor])),
        provideAnimationsAsync()
    ]
})
  .catch(err => console.error(err));
