import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { StorageService } from './storage.service';

@Injectable({
  providedIn: 'root'
})
export class UsersService {

  // currentUserClaims:any = new BehaviorSubject(null)
  justReg:any = new BehaviorSubject(false)
  // currentUser:any = new BehaviorSubject(null)

  constructor(private http : HttpClient, private storage:StorageService) {}

  readonly apiUrl = 'https://localhost:7286/api/'

  getUsers(){
    const token = this.storage.getItem("token")
    const headers = new HttpHeaders({ "Authorization" : `Bearer ${token}`})
    return this.http.get(this.apiUrl + "user/userlist", { headers })
  }

  register(user:any){
    return this.http.post(this.apiUrl + 'Authentication/register', user)
  }

  logIn(user:any){
    return this.http.post(this.apiUrl + 'Authentication/login', user)
  }

  changeMyPassword(user:any){
    const token = this.storage.getItem("token")
    const headers = new HttpHeaders({ "Authorization" : `Bearer ${token}`})
    return this.http.put(this.apiUrl + 'user/changeMyPassword', user, {headers})
  }

  updateUser(user:any){
    const token = this.storage.getItem("token")
    const headers = new HttpHeaders({ "Authorization" : `Bearer ${token}`})
    return this.http.put(this.apiUrl + 'user/' + user.id, user, {headers})
  }

  deleteUser(id:any){
    const token = this.storage.getItem("token")
    const headers = new HttpHeaders({ "Authorization" : `Bearer ${token}`})
    return this.http.delete(this.apiUrl + 'user/' + id, {headers})
  }

  getCurrentClaims(){
    return this.storage.getItem("userClaims")
  }

  getCurrentUser(){
    return this.storage.getItem("user")
  }

  logOut(){
    this.storage.clear()
  }

}
