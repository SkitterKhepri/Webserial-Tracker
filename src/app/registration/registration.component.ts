import { Component, OnDestroy } from '@angular/core';
import { BaseService } from '../services/base.service';
import { UsersService } from '../services/users.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css']
})
export class RegistrationComponent implements OnDestroy {

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
    if(this.passwordCheck(pass1, pass2)){
      this.passMatch = true
      this.newUser.password = pass1
      if (user.email.includes('@') && user.email.includes('.')) {
        this.validEmail = true
        this.userServ.register(user).subscribe({
          next: ()=>{
            this.userServ.justReg.next(true)
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
    else{
      this.passMatch = false
    }
  }

  passwordCheck(pass1:string, pass2:string) : boolean{
    if(pass1 === pass2){
      return true 
    }
    else{ return false }
  }


  ngOnDestroy(): void {
      this.passMatch = true
      this.validEmail = true
  }
}
