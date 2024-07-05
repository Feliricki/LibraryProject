import { Component } from '@angular/core';
import { BookViewComponent } from "../book-view/book-view.component";

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    BookViewComponent
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
}
