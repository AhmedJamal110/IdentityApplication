import { User } from '../shared/modules/account/User';
import { HttpClient, HttpHeaders,  } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Register } from '../shared/modules/account/register';
import { environment } from '../shared/environments/environment.development';
import { login } from '../shared/modules/account/Login';
import { ReplaySubject, map, of } from 'rxjs';
import { Router } from '@angular/router';
import { RegisterWithExternals } from '../shared/modules/account/RegisterWithExternals';
import { ConfirmEmail } from '../shared/modules/account/Confirm-Email';
import { ResetPassword } from '../shared/modules/account/reset-password';
import { LoginWithExternal } from '../shared/modules/account/LoginWithExternal';



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

   registerWiththirdParty(model : RegisterWithExternals){
   return this._HttpClient.post<User>(this.baseUrl+'account/register-with-third-party' , model).pipe( 
    map((user: User) => {
      if(user){
        localStorage.setItem(environment.userkey , JSON.stringify(user));
        this.userSource.next(user);
    }
    })
   )
   }

   loginWithThirdParty(model : LoginWithExternal){
    return this._HttpClient.post<User>(this.baseUrl+'account/login-with-third-party' , model).pipe( 
     map((user: User) => {
       if(user){
         localStorage.setItem(environment.userkey , JSON.stringify(user));
         this.userSource.next(user);
     }
     })
    )
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

  EmailConfirm(model : ConfirmEmail){
   return this._HttpClient.put(this.baseUrl+'Account/confirm-email' , model)
  }




  resendEmailConfirmation(email : string){
    return this._HttpClient.post(this.baseUrl+`Account/resend-email-confirmation-link/${email}` , {})
  }

  ForgetEmailOrPassword(email : string){
    return this._HttpClient.post(this.baseUrl+`Account/forgot-email-or-password${email}` , {})
  }

resetPassword(model : ResetPassword){
  return this._HttpClient.put(this.baseUrl+'Account//reset-password',model)
}


}


