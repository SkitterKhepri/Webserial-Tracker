import { Component } from '@angular/core';
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

  constructor(private userServ:UsersService, private router:Router){}


  register(user:any, pass1:any, pass2:any){
    if(this.passwordMatch(pass1, pass2)){
      this.passMatch = true
      this.newUser.password = pass1
      this.userServ.register(user).subscribe(
        ()=>{
          this.userServ.justReg.next(true)
          this.router.navigate(['/login'])
          this.newUser = {}
        }
      )
      
    }
    else{
      this.passMatch = false
    }
  }

  passwordMatch(pass1:string, pass2:string) : boolean{
    if(pass1 === pass2){
      return true 
    }
    else{ return false }
  }

}
