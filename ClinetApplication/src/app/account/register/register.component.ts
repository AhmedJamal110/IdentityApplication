import { login } from './../../shared/modules/account/Login';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AccountService } from '../account.service';
import { Router } from '@angular/router';
import { SharedService } from 'src/app/shared/shared.service';
import { take } from 'rxjs';
import { User } from 'src/app/shared/modules/account/User';
declare const FB : any;
  

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  submitted = false;

  errorMessages : string[] = [];

  registerFrom : FormGroup = new FormGroup({})

  emailPattern ="^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$"

  
  initialForm(){
    this.registerFrom = this._FormBuilder.group({
      firstName : ['' , [Validators.required , Validators.minLength(3) , Validators.maxLength(15) ]],
      lastName : ['' , [Validators.required , Validators.minLength(3) , Validators.maxLength(15)]],
      email : ['' , [Validators.required , Validators.email , Validators.pattern(this.emailPattern)]],
      paswword : ['' , [Validators.required, Validators.minLength(6) , Validators.maxLength(15) ]],
      
    })
  }
  
  constructor(private _FormBuilder: FormBuilder , private _AccountService:AccountService , 
    private _Router: Router , private _SharedService:SharedService){
      this._AccountService.userSource$.pipe(take(1)).subscribe({
        next : ((user: User | null) => {
          if(user){
            this._Router.navigateByUrl('/')
          }
        })
      })
    }
  
  
  ngOnInit(): void {
  
    this.initialForm()
  }



  onRegisterSubmit(){
    this.submitted = true;

     if(this.registerFrom.valid){
      this._AccountService.register(this.registerFrom.value).subscribe({
        next:(resppnse : any) => {
          this._SharedService.showNotification(true , resppnse.value.title ,resppnse.value.message );
           this._Router.navigateByUrl('account/login') 
        }  ,
    error : (err) => {
      console.log(err);
          if(err.error.errors){
            
            this.errorMessages = err.error.errors;
          }else{
            this.errorMessages.push(err.error)
            
          }
        }
      }) 

  }

  }

  registerWithFacebook(){
  FB.login(async(fbResult : any) => {
    //console.log(fbResult);

    if(fbResult.authResponse){
      
      const accessToken = fbResult.authResponse.accessToken ;
      const userID =  fbResult.authResponse.userID ;
      this._Router.navigateByUrl(`/Account/register/third-party/facebook?access_token=${accessToken}&userid=${userID}`)

    } else{
      this._SharedService.showNotification(false , 'Failed' , 'cant register with facebook')
    }   
  } )
 

}



}


     
  



  
  

 
  
 

    
 