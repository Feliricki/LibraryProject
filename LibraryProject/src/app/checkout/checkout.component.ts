import { Component, OnInit, Signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationService } from "../auth/authentication.service";
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { BooksDto } from "../booksDto";
import { BooksServiceService } from "../book-view/books-service.service";
import { MatListModule } from '@angular/material/list';
import { DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from "@angular/material/button";

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [
    MatListModule, DatePipe, MatCardModule,
    ReactiveFormsModule, MatInputModule, MatFormFieldModule,
    MatButtonModule,
  ],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.css'
})
export class CheckoutComponent implements OnInit {

  book?: BooksDto;
  title: string = "";
  editForm = this.formBuilder.nonNullable.group({
    title: this.formBuilder.nonNullable.control(""),
    description: this.formBuilder.nonNullable.control(""),
  });
  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private authService: AuthenticationService,
    private booksService: BooksServiceService,
    private formBuilder: FormBuilder
  ) {
  }

  ngOnInit() {
    this.getData();
  }

  get Title() {
    return this.editForm.controls.title;
  }

  get Description() {
    return this.editForm.controls.description;
  }

  isAuthenticated(): boolean {
    return this.authService.isAuthenticated();
  }

  isLibrarian(): boolean {
    return this.authService.isLibrarian();
  }

  setCheckoutStatus(checkout: boolean): void {
    if (!this.book) return;
    this.booksService.setCheckoutStatus(this.book.isbn, checkout)
      .subscribe({
        next: changes => {
          if (checkout){
            console.log("Successfully checked out book with " + changes.toString() + " changes");
            // this.router.navigate([this.activatedRoute.snapshot])
          } else {
            console.log(`Successfully checked int book with ${changes} changes`);
          }
        },
        error: err => {
          console.error(err);
        }
      })
  }

  submitEdit(
  ): void {
    if (!this.book) return;
    this.booksService.editBook(
      this.Title.value, this.Description.value, this.book.isbn
    ).subscribe({
      next: changes => {
        console.log(`Made successful edit with ${changes} changes`);
        location.reload();
      },
      error: err => {
        console.error(err);
      }
    })
  }

  deleteBook(): void {
    if (!this.book) return;
    this.booksService.deleteBook(this.book.isbn)
      .subscribe({
        next: changes => {
          console.log(`Made successful deletion with ${changes} changes`);
        },
        error: err => {
          console.error(err);
        }
      })
  }

  getData(){
    let idParams = this.activatedRoute.snapshot.paramMap.get("id");
    let id = idParams ? +idParams : 0;
    console.log("Current isbn is " + id.toString());

    this.booksService.getBook(id)
      .subscribe({
        next: book => {
          this.book = book;
          this.title = "Edit - " + this.book.title;
          this.Title.setValue(this.book.title);
          this.Description.setValue(this.book?.description);
        },
        error: err => {
          console.error(err);
        }
      })
  }
}
