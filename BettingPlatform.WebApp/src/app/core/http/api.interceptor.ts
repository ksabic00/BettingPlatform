import { Injectable } from '@angular/core';
import {
  HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ApiInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((err: HttpErrorResponse) => {
        const p = err.error;
        const messages: string[] = [];
        if (p?.errors) Object.keys(p.errors).forEach(k => messages.push(...p.errors[k]));
        else if (p?.detail) messages.push(p.detail);
        else if (typeof p === 'string') messages.push(p);
        return throwError(() => ({ status: err.status, messages: messages.length ? messages : [err.message] }));
      })
    );
  }
}
