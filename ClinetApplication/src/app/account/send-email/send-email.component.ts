import { Component, OnInit } from '@angular/core';
import { AccountService } from '../account.service';
import { take } from 'rxjs';
import { User } from 'src/app/shared/modules/account/User';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-send-email',
  templateUrl: './send-email.component.html',
  styleUrls: ['./send-email.component.css']
})
export class SendEmailComponent implements OnInit{

  resndEmailForm : FormGroup = new FormGroup({})
  submitted : boolean = false;
  Mode : string | undefined;
  emailPattern ="^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$";
  errorsMessage : string[]  =[];

constructor(private _AccountService: AccountService , private _Router :Router,
  private _ActivatedRoute:ActivatedRoute , private _FormBuilder: FormBuilder , 
    private _SharedService : SharedService      ){}

  ngOnInit(): void {
    this.resendEmailConfirm();
  }

  resendEmailConfirm(){
    this._AccountService.userSource$.pipe(take(1)).subscribe({
      next:((user : User | null) => {
          if(user){
            this._Router.navigateByUrl('/');
          }else{
             const mode = this._ActivatedRoute.snapshot.paramMap.get('mode');
              if(mode){
                this.Mode = mode;
                this.initialForm()
              }
          
            }
      })
    })

  }


  initialForm(){
    this.resndEmailForm = this._FormBuilder.group({
      email : ['' ,[Validators.required, Validators.pattern(this.emailPattern)]]
    })
  }


onSendEmail(){
  this.submitted = true;
  if(this.resndEmailForm.valid && this.Mode ){
      if(this.Mode.includes('resend-email-confirmation-link')){

        this._AccountService.resendEmailConfirmation(this.resndEmailForm.get('email')?.value).subscribe({
          next:(response : any) =>{
            this._SharedService.showNotification(true , response.value.title , response.value.message);
            this._Router.navigateByUrl('/account/login')
          } ,
          error:(err)=> {
            if(err.error.errors){
              this.errorsMessage = err.error.errors;
            }else{
              this.errorsMessage.push(err.error)
            }
          }
        })
      }else if(this.Mode.includes('forgot-email-or-password')){
        this._AccountService.ForgetEmailOrPassword(this.resndEmailForm.get('email')?.value).subscribe({
          next:(response : any) => {
            this._SharedService.showNotification(true , response.value.title , response.value.message);
            this._Router.navigateByUrl('/account/login');
          },
          error:(err)=> {
            if(err.error.errors){
              this.errorsMessage = err.error.errors;
            }else{
              this.errorsMessage.push(err.error)
            }
          }
        })
      }
  }

}


onCancel(){
  this._Router.navigateByUrl('/account/login')
}

}
