import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {BooksDto} from "../booksDto";
import {environment} from "../../environments/environment";
import {ApiResult} from "../checkout/api-result";
import {RouterLink} from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class BooksServiceService {

  constructor(private http: HttpClient) { }

  setCheckoutStatus(isbn: number, checkout: boolean){
    let url = environment.baseUrl + "api/Books/SetCheckout";
    let params = new HttpParams()
      .set("isbn", isbn.toString())
      .set("checkout", checkout ? "true" : "false");

    return this.http.post<number>(url, null, { params: params });
  }

  editBook(title: string, description: string, isbn: number): Observable<number> {
    let url = environment.baseUrl + "api/Books/EditBook";
    let params = new HttpParams()
      .set("title", title)
      .set("description", description)
      .set("isbn", isbn.toString());

    return this.http.post<number>(url, null, { params: params });
  }

  deleteBook(isbn: number): Observable<number> {
    let url = environment.baseUrl + "api/Books/RemoveBook";
    let params = new HttpParams()
      .set("isbn", isbn.toString());

    return this.http.delete<number>(url,  { params: params });
  }

  addBook(book: BooksDto){
    let url = environment.baseUrl + "api/Books/AddBook";
    return this.http.post<number>(url, book);
  }

  getFeaturedBooks(numFeatured: number): Observable<BooksDto[]> {
    let url = environment.baseUrl + 'api/Books/GetFeaturedBooks';
    let parameters = new HttpParams()
      .set("numFeatured", numFeatured.toString());

    return this.http.get<BooksDto[]>(url, { params: parameters });
  }

  getBook(isbn: number){
    let url = environment.baseUrl + "api/Books/GetBook";
    let params = new HttpParams().set("isbn", isbn.toString());
    return this.http.get<BooksDto>(url, { params: params });
  }

  getBooks(
    pageIndex: number = 0,
    pageSize: number = 50,
    sortColumn: string | null = "Title",
    sortOrder: "ASC" | "DESC" | null = "ASC",
    filterColumn: string | null = null,
    filterQuery: string | null = null): Observable<ApiResult<BooksDto>> {

    let parameters = new HttpParams()
      .set("pageIndex", pageIndex.toString())
      .set("pageSize", pageSize.toString());

    if (sortColumn !== null){
      parameters = parameters.set("sortColumn", sortColumn.toString());
    }
    if (sortOrder !== null){
      parameters = parameters.set("sortOrder", sortOrder.toString());
    }
    if (filterColumn !== null && filterQuery !== null){
      parameters = parameters
        .set("filterColumn", filterColumn)
        .set("filterQuery", filterQuery);
    }
    let url = environment.baseUrl + "api/Books/GetBooks";
    return this.http.get<ApiResult<BooksDto>>(url, { params: parameters });
  }
}
