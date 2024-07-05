import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import {AuthenticationService} from "./authentication.service";
import { catchError, Observable, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {

  const authService = inject(AuthenticationService);
  const router = inject(Router);
  const token = authService.getToken();

  console.log("Calling auth interceptor");
  if (token){
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  return next(req).pipe(
    catchError(error => {
      if (error instanceof HttpErrorResponse && error.status === 401){
        authService.logout();
        router.navigate(['login']);
      }
      return throwError(error);
    })
  );
};
