import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AccountService } from '../account.service';
import { User } from 'src/app/shared/modules/account/User';
import { take } from 'rxjs';
import { RegisterWithExternals } from 'src/app/shared/modules/account/RegisterWithExternals';


@Component({
  selector: 'app-register-with-third-party',
  templateUrl: './register-with-third-party.component.html',
  styleUrls: ['./register-with-third-party.component.css']
})
export class RegisterWithThirdPartyComponent implements OnInit {
  
  registerThirdParty : FormGroup = new FormGroup({});
  submitted : boolean = false;
  provider : string | null = null;
  accessToken : string | null = null;
  userid : string | null = null;
  errorMessage : string []  = [];
 
 
  constructor( private _FormBuilder: FormBuilder ,private _AccountService:AccountService,
    private _ActivatedRoute:ActivatedRoute , private _Router: Router){}
 
  ngOnInit(): void {
    this._AccountService.userSource$.pipe(take(1)).subscribe({
      next:(user : User | null) => {
        if(user){
          this._Router.navigateByUrl('/')
        }else{
          this._ActivatedRoute.queryParamMap.subscribe({
            next : (params : any) => {
              this.provider = this._ActivatedRoute.snapshot.paramMap.get('provider');
              this.accessToken = params.get('access_token');
              this.userid = params.get('userid')

                if(this.provider && this.accessToken && this.userid &&
                 (this.provider === 'facebook' || this.provider === 'google'))
                 {
                    this.intialForm()
                }else{
                  this._Router.navigateByUrl('/account/register')
                }
              
            
            }
          })
        }
      }
    })
  
 
  }

intialForm(){
  this.registerThirdParty = this._FormBuilder.group({
    firstName : ['' , [Validators.required , Validators.minLength(3) , Validators.maxLength(15) ]],
    lastName : ['' , [Validators.required , Validators.minLength(3) , Validators.maxLength(15)]]
  
  })
}


onRegiterWithThirdParty(){
  this.submitted = true;
  if(this.registerThirdParty.valid && this.provider && this.accessToken && this.userid){
    const fname = this.registerThirdParty.get('firstName')?.value;
    const lname = this.registerThirdParty.get('lastName')?.value;

    const model = new RegisterWithExternals(fname , lname , this.provider , this.accessToken , this.userid)
 
    this._AccountService.registerWiththirdParty(model).subscribe({
      next : _ => {
        this._Router.navigateByUrl('/')
       },
       error:(err) => {
        if(err.error.errors){
          this.errorMessage = err.error.errors
        }else{
          this.errorMessage.push(err.error)
        }
       }
    })
  
  
  }

}

}
