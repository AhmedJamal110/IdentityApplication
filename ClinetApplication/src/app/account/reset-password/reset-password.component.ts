import { Component, OnInit } from '@angular/core';
import { AccountService } from '../account.service';
import { take } from 'rxjs';
import { User } from 'src/app/shared/modules/account/User';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ResetPassword } from 'src/app/shared/modules/account/reset-password';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit {
    token : string = '';
    email : string = '';
resetPasswordForm : FormGroup = new FormGroup({});

submitted : boolean = false;
errorMessages : string[] =[]

  constructor(private _AccountService: AccountService , private _Router: Router ,
  private _ActivatedRoute: ActivatedRoute , private _FormBuilder: FormBuilder, 
    private _SharedService: SharedService){}
  
  
  ngOnInit(): void {
    this._AccountService.userSource$.pipe(take(1)).subscribe({
      next:((user : User | null) => {
        if(user){
          this._Router.navigateByUrl('/');
        } else{
          this._ActivatedRoute.queryParamMap.subscribe({
            next:((params : any) => {
                this.token = params.get('token');
                this.email = params.get('email');
                if( this.token , this.email){
                  this.initilFrom(this.email)
                }else{
                  this._Router.navigateByUrl('/account/login')
                }
          
            })
          })
        }
      })
    })
  }

 


  initilFrom(userName : string){
    this.resetPasswordForm = this._FormBuilder.group({
      email: [{value : userName , disabled : true}],
      newPassword :['' , [Validators.required , Validators.minLength(3) , Validators.maxLength(15)]]
    })
  }


  onResetPassword(){
    this.submitted = true;
    this.errorMessages = []
if(this.resetPasswordForm.valid , this.token , this.email){
  const resetPass : ResetPassword = {
     token : this.token,
     email : this.email,
     newPassword : this.resetPasswordForm.get('newPassword')?.value
  }

  this._AccountService.resetPassword(resetPass).subscribe({
      next:((response : any) => {
        this._SharedService.showNotification(true , response.value.title , response.value.message)
        this._Router.navigateByUrl('/account/login')
      }),
       error:(err) => {
        if(err.error.errors){
          this.errorMessages = err.error.errores;
        }else{
          this.errorMessages.push(err.error)
        }
       }
  })
}


  }

}
