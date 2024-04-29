import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Register } from '../shared/modules/register';
import { environment } from '../shared/environments/environment.development';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  constructor(private http : HttpClient) { }

  register(model : Register):Observable<any>
  {
    return this.http.post(`${environment.apiUrl}/api/Account/register`, model)
  }

}
