import { Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';



@Component({
  selector: 'app-notifacation',
  templateUrl: './notifacation.component.html',
  styleUrls: ['./notifacation.component.css']
})
export class NotifacationComponent {

  isSuccess : boolean = true;
  title : string = '';  
  message : string = '';

  constructor(public _BsModalRef: BsModalRef){}

}
