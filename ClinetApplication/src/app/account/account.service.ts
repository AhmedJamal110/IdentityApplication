import { User } from './../shared/modules/User';
import { HttpClient, HttpHeaders,  } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Register } from '../shared/modules/register';
import { environment } from '../shared/environments/environment.development';
import { login } from '../shared/modules/Login';
import { ReplaySubject, map, of } from 'rxjs';
import { Router } from '@angular/router';



@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseUrl = environment.apiUrl;
  
  private userSource = new ReplaySubject<User | null>(1)
  userSource$ = this.userSource.asObservable();

  constructor(private _HttpClient: HttpClient , private _Router:Router){}
 

  getJwt(){
    const token = localStorage.getItem(environment.userkey);
    if(token){
      const user: User = JSON.parse(token)
      return user.token;
    } else{
      return null
    }
  }

  refreshUser(token : string | null ){
    if(token === null){
      this.userSource.next(null);
      return of(undefined)
    }else{
      let headers = new HttpHeaders();
      headers = headers.set('Authorization', 'Bearer '+token)
      return this._HttpClient.get<User>(this.baseUrl+'Account/refresh-user-token',{headers}).pipe(
        map((user:User) => {
          localStorage.setItem(environment.userkey , JSON.stringify(user));
          this.userSource.next(user);
        })
      )
    }
  }


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

  logout(){
    localStorage.removeItem(environment.userkey);
    this.userSource.next(null);
    this._Router.navigateByUrl('/')
  }


}
