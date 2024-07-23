import { Component, OnInit, WritableSignal, signal } from '@angular/core';
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
import { reviewFormValidator } from './reviewValidator';

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

  // TODO: Consider adding some custom validation.
  reviewForm = this.formBuilder.nonNullable.group({
    score: this.formBuilder.control(null as null | number, { validators: reviewFormValidator() })
  });

  finishedLoading: boolean = false;
  constructor(
    private activatedRoute: ActivatedRoute,
    private authService: AuthenticationService,
    private booksService: BooksServiceService,
    private formBuilder: FormBuilder
  ) {
  }

  ngOnInit() {
    this.getData();
  }

  get Score() {
    return this.reviewForm.controls.score;
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

  leaveReview(): void {
    if (!this.book) return;
    const user = this.authService.currentUser();
    if (!user) return;
    const score = this.reviewForm.controls.score.value;
    if (score === null) return;
    if (this.reviewForm.invalid) return;

    this.booksService.leaveReview(this.book.isbn, score, user)
      .subscribe({
        next: changes => {
          console.log(`Successfully made ${changes} changes when leaving a review.`);
        },
        error: err => console.error(err)
      });
  }

  setCheckoutStatus(checkout: boolean): void {
    if (!this.book) return;
    this.booksService.setCheckoutStatus(this.book.isbn, checkout)
      .subscribe({
        next: changes => {
          if (checkout) {
            console.log("Successfully checked out book with " + changes.toString() + " changes");
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
          location.reload();
        },
        error: err => {
          console.error(err);
        }
      })
  }

  getData() {
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
          this.finishedLoading = true;
        },
        error: err => {
          console.error(err);
          this.finishedLoading = true;
        },
      })
  }
}
