import { Component } from '@angular/core';
import { BaseService } from '../base.service';
import { UsersService } from '../users.service';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css']
})
export class RegistrationComponent {

  
  users:any = []
  newUser:any = {
    "id" : 0,
    "userName" : "",
    "email" : "",
    "password" : ""
  }

  constructor(private userServ:UsersService){
    this.getUsers()
  }

  getUsers(){
    this.userServ.getUsers().subscribe(
      (users:any)=> this.users = users
      )
  }

  postNewUser(userName:string, email:string, pass1:string, pass2:string){
    //this might not work, have possible saved solution in project folder
    if (this.users.findIndex((u:any) => u.userName === this.newUser.userName) === -1) {
      if(this.passwordMatch(pass1, pass2)){
        this.newUser.userName = userName
        this.newUser.email = email
        this.newUser.password = pass1
        console.log(this.newUser)
        this.userServ.postUser(this.newUser).subscribe();
        console.log("User added")
      }
      else{
        console.log("Passwords dont match")
      }
    }
    else{
      console.log("Username taken")
    }
  }

  passwordMatch(pass1:string, pass2:string) : boolean{
    if(pass1 === pass2){
      return true 
    }
    else{ return false }
  }

  kiir(){
    this.userServ.postUser({
      "id": 0,
      "userName": "idegen",
      "email": "idegen@gmail.com",
      "password": "ass1"
    }).subscribe()
  }

}
