import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { StorageService } from './storage.service';
import { Router } from '@angular/router';
import { UsersService } from './users.service';


@Injectable({
  providedIn: 'root'
})
export class AuthServiceService {

  private seessionTime = 10800000 //3 * 60 * 60 * 1000 -- 3 hours in miliseconds

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

  
  isSessionExpired(): boolean {
    const loginTime = this.storage.getItem('loginTime');
    if (loginTime) {
      const currentTime = new Date().getTime();
      const timeElapsed = currentTime - JSON.parse(loginTime);

      if (timeElapsed > this.seessionTime) {
        return true;
      }
    }
    return false;
  }
}
