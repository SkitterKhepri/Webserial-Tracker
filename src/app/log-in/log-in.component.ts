import { HttpClient } from '@angular/common/http';
import { Component, OnDestroy } from '@angular/core';
import { BaseService } from '../services/base.service';
import { UsersService } from '../services/users.service';
import { StorageService } from '../services/storage.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-log-in',
  templateUrl: './log-in.component.html',
  styleUrls: ['./log-in.component.css']
})
export class LogInComponent implements OnDestroy {

  justReg:boolean = false
  errorMessage:String = ""
  isLoading:boolean = false

  constructor(private userServ:UsersService, private http:HttpClient, private storage:StorageService, private router:Router){
    this.userServ.justReg.subscribe((reg:any) => this.justReg = reg)
  }

  login(user:any){
    this.isLoading = true
    this.userServ.logIn(user).subscribe({
      next: (loggedUser:any) => {
        this.storage.setItem("token", loggedUser.token)
        this.router.navigate(['/home'])
        this.storage.setItem("userClaims", JSON.stringify(loggedUser.roles))
        this.storage.setItem("user", loggedUser.user)
        this.storage.setItem("loginTime", JSON.stringify(new Date().getTime()))
        this.isLoading = false
      },
      error: (error) => {
        this.errorMessage = error.message
        this.isLoading = false
      },
      complete: () => {
        this.isLoading = false
      }
      
    })
  }

  ngOnDestroy(): void {
      this.userServ.justReg.next(false)
  }
}
