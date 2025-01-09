import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { StorageService } from './storage.service';
import { Router } from '@angular/router';
import { UsersService } from './users.service';


@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private seessionTime = 10800000 //3 * 60 * 60 * 1000 -- 3 hours in miliseconds

  private readonly apiUrl = 'https://localhost:7286/api/'

  constructor(private http : HttpClient, private storage:StorageService, private route : Router, private userServ : UsersService) { }

  getCurrentClaims(){
    return JSON.parse(this.storage.getItem("userClaims"))
  }

  getCurrentUser(){
    return this.storage.getItem("user")
  }

  logOut(){
    this.storage.clear()
    this.route.navigate(['/home'])
  }

  logoutGoLogin(){
    this.storage.clear()
    this.route.navigate(['/login'], {state: {sessionExp : true}})
  }

  noAdminClaims(){
    this.route.navigate(['/home'])
  }
  
  isSessionExpired(): boolean {    
    const loginTime = this.storage.getItem('loginTime');
    if (loginTime == null){
        return true
      }
    if (loginTime != null) {
      const currentTime = new Date().getTime();
      const timeElapsed = currentTime - JSON.parse(loginTime);
      if (timeElapsed > this.seessionTime) {
        return true;
      }
    }
    return false;
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
