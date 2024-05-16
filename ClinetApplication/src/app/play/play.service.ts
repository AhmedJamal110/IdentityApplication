import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../shared/environments/environment.development';



@Injectable({
  providedIn: 'root'
})
export class PlayService {

baseUrl = environment.apiUrl;

constructor( private _HttpClient:HttpClient) { }

getPlayers(){
  return this._HttpClient.get(this.baseUrl+'play/get-players')
}

}
