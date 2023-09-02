import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, delay, finalize } from 'rxjs';
import { BusyService } from '../services/busy.service';

// During development everything behaves very fast since the site is local. Insert a fake delay to simulate normal browsing experience and have the opportunity to view bugs and artifacts.
@Injectable()
export class LoadingInterceptor implements HttpInterceptor {

  constructor(private busyService: BusyService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    this.busyService.busy();
    return next.handle(request).pipe(
      delay(1000),
      finalize(() => this.busyService.idle())
    );
  }
}
