import { ConfirmEmail } from './../../shared/modules/account/Confirm-Email';
import { User } from 'src/app/shared/modules/account/User';
import { Component, OnInit } from '@angular/core';
import { take } from 'rxjs';
import { AccountService } from '../account.service';
import { ActivatedRoute, Router } from '@angular/router';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-email-confirm',
  templateUrl: './email-confirm.component.html',
  styleUrls: ['./email-confirm.component.css']
})
export class EmailConfirmComponent implements OnInit {

  success :boolean =true;

constructor(private _AccountService: AccountService , private _Router: Router ,
  private _ActivatedRoute: ActivatedRoute , private _SharedService: SharedService
){}
 

ngOnInit(): void {
    this._AccountService.userSource$.pipe(take(1)).subscribe({
      next :((user : User | null) => {
        if(user){
            this._Router.navigateByUrl('/')
        }else{
          this._ActivatedRoute.queryParamMap.subscribe({
            next:(params: any) => {
              const confirmEmail : ConfirmEmail ={
                token : params.get('token'),
                email : params.get('email')
              }  
              this._AccountService.EmailConfirm(confirmEmail).subscribe({
                next :(response : any) => {
              this._SharedService.showNotification(true ,response.value.title, response.value.message )
                },
                error:(err) => {
                  this.success = true;
                  this._SharedService.showNotification(false , "Faild" , err.error)
                }
                
              })



            }
          })

        }
      })
    })

  }



  resendEmailConfirmtion(){
    this._Router.navigateByUrl('/account/send-email/resend-email-confirmation-link')
  }


}
