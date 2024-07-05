import { Component, OnInit, ViewChild } from '@angular/core';
import { MatCardModule } from '@angular/material/card'
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatTabsModule } from '@angular/material/tabs';
import { MatListModule } from '@angular/material/list';
import { AsyncPipe } from '@angular/common';
import { BooksServiceService } from "./books-service.service";
import { BooksDto } from "../booksDto";
import { MatIconModule } from '@angular/material/icon';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatPaginatorModule, MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { RouterLink, Router } from '@angular/router';
import {AuthenticationService} from "../auth/authentication.service";

type BookColumns =
  "Title" |
  "Author" |
  "Available";

@Component({
  selector: 'app-book-view',
  standalone: true,
  imports: [
    MatCardModule, MatButtonModule, AsyncPipe,
    MatTableModule, MatTabsModule, MatListModule,
    MatIconModule, MatSortModule, MatPaginatorModule,
    MatFormFieldModule, MatInputModule, ReactiveFormsModule,
    RouterLink,
  ],
  templateUrl: './book-view.component.html',
  styleUrl: './book-view.component.css'
})
export class BookViewComponent implements OnInit {

  featuredBooks: BooksDto[] = [];
  tableBooks: BooksDto[] = [];

  readonly displayedColumns: string[] = [
    "CoverImage", "Title", "Descriptions", "Author", "Availability"
  ];

  defaultPageIndex: number = 0;
  defaultPageSize: number = 10;

  defaultSortColumn: BookColumns = "Title";
  defaultSortOrder: "ASC" | "DESC" = "ASC";
  defaultFilterColumn = "Title";

  currentSortColumn: BookColumns = "Title";
  currentSortOrder: "ASC" | "DESC" = "ASC";

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  searchForm = this.formBuilder.nonNullable.group({
    search: this.formBuilder.nonNullable.control(""),
  });

  newBookForm = this.formBuilder.nonNullable.group({
    title: this.formBuilder.nonNullable.control(""),
    bookCoverUrl: this.formBuilder.nonNullable.control(""),
    description: this.formBuilder.nonNullable.control(""),
    author: this.formBuilder.nonNullable.control(""),
    publisher: this.formBuilder.nonNullable.control(""),
    category: this.formBuilder.nonNullable.control(""),
    pageCount: this.formBuilder.nonNullable.control(1),
  });

  filterTextChanged: Subject<string> = new Subject<string>();
  constructor(
    private booksService: BooksServiceService,
    private authService: AuthenticationService,
    private formBuilder: FormBuilder,
    private router:Router) {
  }

  ngOnInit() {
    this.getFeaturedBooks();
    this.getBooks(
      this.defaultPageIndex,
      this.defaultPageSize,
      this.defaultSortColumn,
      this.defaultSortOrder,
      null,
      null
    );
  }

  addNewBook(){
    const today = new Date();
    console.log(this.newBookForm);
    this.booksService.addBook({
      title: this.newBookForm.controls.title.value,
      bookCoverUrl: this.newBookForm.controls.bookCoverUrl.value,
      description: this.newBookForm.controls.description.value,
      author: this.newBookForm.controls.author.value,
      publisher: this.newBookForm.controls.publisher.value,
      available: true,
      daysUntilAvailable: 0,
      publicationDate: today,
      category: this.newBookForm.controls.category.value,
      isbn: 0,
      pageCount:this.newBookForm.controls.pageCount.value
    }).subscribe({
      next: response =>{
        console.log("successfully added book");
        location.reload();
      },
      error: err => {
        console.error(err);
      }
    })

  }

  isLibrarian(): boolean {
    return this.authService.isLibrarian();
  }

  clearInput() {
    this.Search.setValue("");
    this.filterTextChanged.next("");
  }
  get Search() {
    return this.searchForm.controls.search;
  }

  filterEvent(query?: string) {
    if (!this.filterTextChanged.observed){
      this.filterTextChanged.pipe(
        debounceTime(1000),
        distinctUntilChanged()
      ).subscribe({
        next: query => {
          this.getBooks(
            this.defaultPageIndex,
            this.defaultPageSize,
            this.defaultSortColumn,
            this.defaultSortOrder,
            "Title",
            query
          );
        }
      });
    }

    this.filterTextChanged.next(query ?? "");
  }

  sortChange(sort: Sort){
    let column: BookColumns = "Title";
    switch (sort.active){
      case "Author":
        column = "Author";
        break;
      case "Available":
        column = "Available";
        break;
    }

    let sortOrder: "ASC" | "DESC" = sort.direction === "asc" ? "ASC" : "DESC";

    this.currentSortColumn = column;
    this.currentSortOrder = sortOrder;

    this.getBooks(
      this.defaultPageIndex,
      this.defaultPageSize,
      column,
      sortOrder,
      this.defaultFilterColumn,
      null
    )
  }

  paginatorChange(event: PageEvent){
    this.getBooks(
      event.pageIndex,
      event.pageSize,
      this.currentSortColumn,
      this.currentSortOrder,
      this.defaultFilterColumn,
      this.Search.value,
    );
  }

  getFeaturedBooks(numBooks: number = 10): void {
    this.booksService.getFeaturedBooks(numBooks)
      .subscribe({
        next: books => {
          this.featuredBooks = books;
        },
        error: err => {
          console.error(err);
        }
      });
  }

  getBooks(
    pageIndex: number = 10,
    pageSize: number = 50,
    sortColumn: string = "Title",
    sortOrder: "ASC" | "DESC" = "ASC",
    filterColumn: string | null = null,
    filterQuery: string | null = null
  ){
    this.booksService.getBooks(
      pageIndex,
      pageSize,
      sortColumn,
      sortOrder,
      filterColumn,
      filterQuery
    )
      .subscribe({
        next: apiResult => {
          this.paginator.length = apiResult.totalCount;
          this.paginator.pageIndex = apiResult.pageIndex;
          this.paginator.pageSize = apiResult.pageSize;

          this.tableBooks = apiResult.data;
        },
        error: err => {
          console.error(err);
        }
      });
  }

}
