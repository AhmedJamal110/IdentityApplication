import { User } from '../../shared/modules/account/User';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AccountService } from '../account.service';
import { ActivatedRoute, Router } from '@angular/router';
import { SharedService } from 'src/app/shared/shared.service';
import { take } from 'rxjs';
import { LoginWithExternal } from 'src/app/shared/modules/account/LoginWithExternal';

declare const FB : any;



@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  loginForm : FormGroup = new FormGroup({})
  emailPattern = "^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$"
  errorMessages : string[] = [];
  subnitted : boolean = false;
  retutnUrl : string | null = null;

constructor(private _formBuilder : FormBuilder  , 
  private _AccountService: AccountService  , private _Router: Router
   , private _SharedService: SharedService , private _ActivatedRoute: ActivatedRoute )
  { this._AccountService.userSource$.pipe(take(1)).subscribe({
    next:(user : User | null) => {
      if(user){
        this._Router.navigateByUrl('/')
      }else{
        this._ActivatedRoute.queryParamMap.subscribe({
          next : (params : any) => {
            if(params) {
             this.retutnUrl =params.get('returnUrl')
            }
          }
        })
      }
    } 
  }) }


ngOnInit(): void {
    this.initialFrom()
  }
 

  initialFrom(){
    this.loginForm = this._formBuilder.group({
      email : ['' , [Validators.required , Validators.pattern(this.emailPattern)]],
      password : ['' , [Validators.required , Validators.minLength(3) , Validators.maxLength(16)]]
    })
  }


  onLoginFrom(){
    this.subnitted = true;
     if(this.loginForm.valid){  
    this._AccountService.login(this.loginForm.value).subscribe({
      next:_ => {
        if(this.retutnUrl ){
          this._SharedService.showNotification(true , 'Welcom in PLAY Page', '')
          this._Router.navigateByUrl(this.retutnUrl);
        }else{
          this._SharedService.showNotification(true , 'Welcom in Home Page', '')
         this._Router.navigateByUrl('/')
        }
        
      },
      error :(err) => {
        console.log(err)
        if(err.error.errors){
            this.errorMessages = err.error.errors;
        }else{
          this.errorMessages.push(err.error)
        }
        
      } 
     })
  }


  }

  loginWithFacebook(){
    FB.login(async(fbResult : any) => {
  
      if(fbResult.authResponse){
        
        const accessToken = fbResult.authResponse.accessToken ;
        const userID =  fbResult.authResponse.userID;

        const model = new LoginWithExternal(accessToken , userID , "facebook")
        this._AccountService.loginWithThirdParty(model).subscribe({
          next: _=> {
            if( this.retutnUrl){
              this._SharedService.showNotification(true , 'Welcom in PLAY Page', '')
              this._Router.navigateByUrl(this.retutnUrl);
            }else{
              this._Router.navigateByUrl('/')
            }
          },
          error :(err) => {
            console.log(err)
            if(err.error.errors){
                this.errorMessages = err.error.errors;
            }else{
              this.errorMessages.push(err.error)
            }
            
          } 
        })
  
      } else{
        this._SharedService.showNotification(false , 'Failed' , 'cant register with facebook')
      }   
    } )
  }

  resendEmailConfirm(){

    this._Router.navigateByUrl('/account/send-email/resend-email-confirmation-link')
  }

}
