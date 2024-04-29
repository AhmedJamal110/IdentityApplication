import { AccountService } from './../account.service';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
 
  registerForm : FormGroup = new FormGroup({});
 submitted = false;
 errorsMessage : string[] = []; 

  constructor( private _AccountService :AccountService ,private FormBuilder : FormBuilder ){}
  ngOnInit(): void {
    this.InitialaizeForm();
 }

  InitialaizeForm(){
    this.registerForm = this.FormBuilder.group({
      firstName :['' , Validators.required , Validators.minLength(3) , Validators.maxLength(15)],
      lastName :['' , Validators.required , Validators.minLength(3) , Validators.maxLength(15)],
      email :['' , Validators.required , Validators.pattern('^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$"')],
      password :['' , Validators.required , Validators.minLength(6) , Validators.maxLength(15)],    
    
    })
  }
 
  register(){

    this.submitted= true;
    this.errorsMessage = [];

    this._AccountService.register(this.registerForm.value).subscribe({
      next:(response) => {
        console.log(response);
        
      },
      error : error =>{
        console.log(error);
        
      }
    })
  }
}
