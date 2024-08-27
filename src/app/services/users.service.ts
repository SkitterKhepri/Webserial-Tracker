import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UsersService {

  constructor(private http : HttpClient) {
    this.getUsers()
  }

  readonly apiUrl = 'https://localhost:7286/api/Users/'

  getUsers() : Observable<any>{
    return this.http.get(this.apiUrl)
  }

  postUser(user: any): Observable<any> {
    return this.http.post(this.apiUrl, user);
  }
  
  putUser(user:any){
    return this.http.put(this.apiUrl + user.id, user)
  }

  deleteUser(id:any){
    return this.http.delete(this.apiUrl + id)
  }
}
