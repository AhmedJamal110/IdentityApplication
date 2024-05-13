import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { NotifacationComponent } from './components/models/notifacation/notifacation.component';


@Injectable({
  providedIn: 'root'
})
export class SharedService {

  BsModalRef?: BsModalRef
  
  constructor(private _BsModalService:BsModalService ) { }
  
showNotification(isSuccess : boolean , title : string ='' , message : string){

 const initialState : ModalOptions ={
    initialState :{
      isSuccess,
      title,
      message
     }
 }  

 this.BsModalRef = this._BsModalService.show(NotifacationComponent , initialState)

}





}
