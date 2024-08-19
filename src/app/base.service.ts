import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BaseService {

  constructor(private http : HttpClient) {
    this.getUsers()
   }

  readonly apiUrl = 'https://localhost:7286/api/'
  private userSub = new BehaviorSubject([])

  async getUsers() : Promise<Observable<any>>{
    return this.http.get(this.apiUrl + "Users")
  }

  postUser(user: any): Observable<any> {
    return this.http.post(this.apiUrl + "Users", user);
  }
  

  putUser(user:any){
    return this.http.put(this.apiUrl + "Users/" + user.id, user)
  }

  deleteUser(id:any){
    this.http.delete(this.apiUrl + "Users/" + id)
  }

}
