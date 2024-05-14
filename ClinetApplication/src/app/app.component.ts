import { Component, OnInit } from '@angular/core';
import { AccountService } from './account/account.service';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  title = 'ClinetApplication';


  constructor(private _AccountService: AccountService){}
  ngOnInit(): void {
    this.redershUser()
  
  }

redershUser(){
  const token =this._AccountService.getJwt()
  if(token){
    this._AccountService.refreshUser(token).subscribe({
      next: () => {},
      error:(err) => {
        this._AccountService.logout()
      }
    
    })
  }else{
    this._AccountService.refreshUser(null).subscribe()
  }

}

}
