import { Component, OnDestroy } from '@angular/core';
import { BaseService } from '../services/base.service';
import { UsersService } from '../services/users.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css']
})
export class RegistrationComponent {

  newUser:any = {
    "username" : "",
    "email" : "",
    "password" : ""
  }
  passMatch:boolean = true
  validEmail:boolean = true
  errorMessage:string = ""
  isLoading:boolean = false

  constructor(private userServ:UsersService, private router:Router){}


  register(user:any, pass1:any, pass2:any){
    this.isLoading = true
    this.passwordCheck(pass1, pass2)
    if(this.passMatch){
      this.newUser.password = pass1
      if (user.email.includes('@') && user.email.includes('.')) {
        this.validEmail = true
        this.userServ.register(user).subscribe({
          next: ()=>{
            this.userServ.justReg.next("reg")
            this.router.navigate(['/login'])
            this.newUser = {}
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
      else{
        this.validEmail = false
      }
    }
  }

  passwordCheck(pass1:string, pass2:string){
    if(pass1 === pass2){
      this.passMatch = true 
    }
    else{ this.passMatch = false }
  }
}
