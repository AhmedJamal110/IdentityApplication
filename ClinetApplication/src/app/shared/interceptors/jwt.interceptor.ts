 import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, take } from 'rxjs';
import { AccountService } from 'src/app/account/account.service';
import { User } from '../modules/User';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private _AccountService: AccountService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    this._AccountService.userSource$.pipe(take(1)).subscribe({
      next:(user) => {
        request = request.clone({
          setHeaders : {
            Authorization : `Bearer ${user?.token}`
          }
        })
      }
    })
   
    return next.handle(request);
  }
}
