import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router:Router, private toastr:ToastrService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Manage the observable from handle(request) using the RxJS library.
    // To use the operators and functions of RxJS, we use the pipe() method and operate on the observable it returns.
    // Operating on the observable from pipe() is done before passing it back to any of our components.
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error) {
          if (error.status === 400) {
            if (error.error.errors) { // validation errors are represented by an array of error messages 
              
              // Let the component deal with the validation errors. 
              // Usually some info is missing from a form and we want the form's component to handle that.
              throw error.error; 
            } else {
              this.toastr.error(error.error.message, error.status.toString());
            }
          }
          if (error.status === 401) {
            this.toastr.error(error.error.message, error.status.toString())
          }
          if (error.status === 404) {
            this.router.navigateByUrl('/not-found');
          }
          if (error.status === 500) {
            // Use NavigationExtras to pass state to the component we route to
            const navExtras: NavigationExtras = {state: {error: error.error}};
            this.router.navigateByUrl('/server-error', navExtras);
          }
        }
        return throwError(() => new Error(error.message));
      })
    );
  }
}
