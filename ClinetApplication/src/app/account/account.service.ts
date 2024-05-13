import { User } from './../shared/modules/User';
import { HttpClient,  } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Register } from '../shared/modules/register';
import { environment } from '../shared/environments/environment.development';
import { login } from '../shared/modules/Login';
import { ReplaySubject, map } from 'rxjs';
import { Router } from '@angular/router';



@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseUrl = environment.apiUrl;
  
  private userSource = new ReplaySubject<User | null>(1)
  userSource$ = this.userSource.asObservable();

  constructor(private _HttpClient: HttpClient , private _Router:Router){}
 

  register(model : Register){
    return this._HttpClient.post<User>(this.baseUrl+'Account/register',model)
   }

  login(model : login){
    return this._HttpClient.post<User>(this.baseUrl+'Account/login',model).pipe(
      map( (user : User) => {
        if(user){
          localStorage.setItem(environment.userkey , JSON.stringify(user))
          this.userSource.next(user)
        }
      })
    ) 
  }




}
