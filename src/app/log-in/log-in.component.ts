import { HttpClient } from '@angular/common/http';
import { Component, OnDestroy } from '@angular/core';
import { BaseService } from '../services/base.service';
import { UsersService } from '../services/users.service';
import { StorageService } from '../services/storage.service';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth-service.service';
import { BehaviorSubject } from 'rxjs';

@Component({
  selector: 'app-log-in',
  templateUrl: './log-in.component.html',
  styleUrls: ['./log-in.component.css']
})
export class LogInComponent implements OnDestroy {

  redirectFrom:string = ""
  errorMessage:String = ""
  isLoading:boolean = false
  sessionExp:boolean = false

  sendingEmail:boolean = false
  fError:string = ""

  constructor(private userServ:UsersService, private http:HttpClient, private storage:StorageService, private router:Router, private authServ:AuthService){
    this.userServ.justReg.subscribe((reg:any) => this.redirectFrom = reg)
    const navigation = router.getCurrentNavigation()
    if (navigation?.extras.state){
      this.sessionExp = navigation.extras.state['sessionExp']
    }
  }

  login(user:any){
    this.isLoading = true
    this.userServ.logIn(user).subscribe({
      next: (loggedUser:any) => {
        this.storage.setItem("token", loggedUser.token)
        this.storage.setItem("userClaims", loggedUser.roles)
        this.storage.setItem("user", loggedUser.user)
        this.storage.setItem("loginTime", new Date().getTime())
        console.log(loggedUser.likes)
        this.storage.setItem("likes", loggedUser.likes)
        this.isLoading = false
        this.router.navigate(['/home'])
      },
      error: (error:any) => {
        this.errorMessage = error.message
        this.isLoading = false
      },
      complete: () => {
        this.isLoading = false
        this.authServ.loginEvent.next(true)
      }
      
    })
  }

  forgotten(email:string){
    this.sendingEmail = true
    this.authServ.resetPassReq(email).subscribe({
      error: (error:any) => {
        this.fError = error.message
        this.sendingEmail = false
      },
      complete: () => this.sendingEmail = false
    })
  }

  ngOnDestroy(): void {
      this.userServ.justReg.next("")
  }
}
