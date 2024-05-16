import {  ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable , map} from 'rxjs';
import { AccountService } from 'src/app/account/account.service';
import { SharedService } from '../shared.service';
import { User } from '../modules/User';


@Injectable({
  providedIn: 'root'
})
export class AuthorizationGuard  {
  constructor( private _AccountService: AccountService , 
    private _SharedService: SharedService , private _Router: Router  ) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot ): Observable<boolean> {
    return this._AccountService.userSource$.pipe(
      map((user : User | null) => {
        if(user){
          return true
        }else{
          this._SharedService.showNotification(false , 'Restricted Area', 'Leave Immediately!')
          this._Router.navigate(['account/login'], {queryParams : {returnUrl : state.url}} )
          return false;
        }
      })
    )

  }
}
