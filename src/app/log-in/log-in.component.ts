import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { BaseService } from '../base.service';
import { UsersService } from '../users.service';

@Component({
  selector: 'app-log-in',
  templateUrl: './log-in.component.html',
  styleUrls: ['./log-in.component.css']
})
export class LogInComponent {

  users:any = []
  newUser:any

  constructor(private userServ:UsersService, private http:HttpClient){
    this.getUsers()
  }

  getUsers(){
    this.userServ.getUsers().subscribe(
      (users:any)=> this.users = users
      )
  }

  //this for register component
  postNewUser(){
    this.userServ.postUser(this.newUser)
  }
}
