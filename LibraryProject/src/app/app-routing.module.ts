import { NgModule } from '@angular/core';
import { Routes, RouterModule } from "@angular/router";
import { HomeComponent } from "./home/home.component";
import {LoginComponent} from "./auth/login/login.component";
import {SignupComponent} from "./auth/signin/signup.component";
import {CheckoutComponent} from "./checkout/checkout.component";

const routes: Routes = [
  { path: "", component: HomeComponent, pathMatch: "full"},
  { path: "login", component: LoginComponent },
  { path: "signup", component: SignupComponent },
  { path: "checkout/:id", component: CheckoutComponent }
  // { path: "**", component: PageNotFoundComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
