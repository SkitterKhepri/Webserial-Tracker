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

  constructor(private userServ:UsersService, private http:HttpClient, private storage:StorageService, private router:Router){
    this.userServ.justReg.subscribe((reg:any) => this.justReg = reg)
  }

  login(user:any){
    this.userServ.logIn(user).subscribe(
      (loggedUser:any) => {this.storage.setItem("token", loggedUser.token)
        if(this.storage.getItem("token") !== null){
          this.router.navigate(['/home'])
        }
        this.storage.setItem("userClaims", loggedUser.roles)
        this.storage.setItem("user", loggedUser.user)
      }
    )
  }

  ngOnDestroy(): void {
      this.userServ.justReg.next(false)
  }
}
