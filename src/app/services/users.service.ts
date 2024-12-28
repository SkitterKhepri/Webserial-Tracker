import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs';
import { StorageService } from './storage.service';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class UsersService {

  justReg:any = new BehaviorSubject(false)

  constructor(private http : HttpClient, private storage:StorageService) {}

  readonly apiUrl = 'https://localhost:7286/api/'

  getUsers(){
    const token = this.storage.getItem("token")
    const headers = new HttpHeaders({ "Authorization" : `Bearer ${token}`})
    return this.http.get(this.apiUrl + "user/userlist", { headers })
  }

  register(user:any){
    return this.http.post(this.apiUrl + 'Authentication/register', user).pipe(catchError(this.handleError))
  }

  logIn(user:any){
    return this.http.post(this.apiUrl + 'Authentication/login', user).pipe(catchError(this.handleError))
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

  private handleError(error: HttpErrorResponse) {
    let errorMessage:any = "An unknown error occurred!";
    if (error.status === 400) {
      (typeof(error.error) == "string") ? errorMessage = error.error : errorMessage = "Bad Request: Invalid data.";
    }
    return throwError(() => new Error(errorMessage));
  }

  resetPassReq(id:string){
    const token = this.storage.getItem("token")
    const headers = new HttpHeaders({ "Authorization" : `Bearer ${token}`})
    return this.http.head(this.apiUrl + "user/password/resetReq/" + id, {headers})
  }

  resetPassword(resetPassDTO:any, id:string){
    const token = this.storage.getItem("token")
    const headers = new HttpHeaders({ "Authorization" : `Bearer ${token}`})
    return this.http.post(this.apiUrl + "user/password/reset/" + id, resetPassDTO, {headers})
  }
}

