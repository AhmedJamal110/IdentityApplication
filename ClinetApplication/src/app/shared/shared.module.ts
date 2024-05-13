import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotfoundComponent } from './components/errors/notfound/notfound.component';
import { ValidationMessagesComponent } from './components/errors/validation-messages/validation-messages.component';
import { RouterModule } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { TextInputComponent } from './components/text-input/text-input.component';
import { NotifacationComponent } from './components/models/notifacation/notifacation.component';
import { ModalModule } from 'ngx-bootstrap/modal';


@NgModule({
  declarations: [
    NotfoundComponent,
    ValidationMessagesComponent,
    TextInputComponent,
    NotifacationComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    HttpClientModule,
    ModalModule.forRoot()
  ],
  exports:[
    RouterModule,
    ReactiveFormsModule,
    HttpClientModule,
    TextInputComponent,
    ValidationMessagesComponent
  ]
})
export class SharedModule { }
