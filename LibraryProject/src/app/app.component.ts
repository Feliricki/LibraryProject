import { Component } from '@angular/core';
import { RouterOutlet } from "@angular/router";
import { NavBarComponent } from "./nav-bar/nav-bar.component";

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css'],
    standalone: true,
    imports: [
      RouterOutlet,
      NavBarComponent,
    ]
})
export class AppComponent {
  constructor() {}

  title: string = 'LibraryProject';
}
