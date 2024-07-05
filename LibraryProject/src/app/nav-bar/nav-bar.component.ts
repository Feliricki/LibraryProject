import { Component } from '@angular/core';
import { MatToolbarModule } from "@angular/material/toolbar";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { RouterLink, Router } from '@angular/router';
import {AuthenticationService} from "../auth/authentication.service";

@Component({
  selector: 'app-nav-bar',
  standalone: true,
  imports: [
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    RouterLink
  ],
  templateUrl: './nav-bar.component.html',
  styleUrl: './nav-bar.component.css'
})
export class NavBarComponent {

  constructor(
    private authService: AuthenticationService,
    private router: Router,
  ) {
  }

  isAuthenticated(): boolean {
    return this.authService.isAuthenticated();
  }

  isLibrarian(): boolean {
    return this.authService.isLibrarian();
  }

  logout(): void {
    this.authService.logout();
    location.reload();
  }

}
