import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { BaseService } from '../base.service';

@Component({
  selector: 'app-log-in',
  templateUrl: './log-in.component.html',
  styleUrls: ['./log-in.component.css']
})
export class LogInComponent {

  users:any = []
  newUser:any

  constructor(private base:BaseService, private http:HttpClient){
    this.getUsers()
  }

  async getUsers(){
    (await this.base.getUsers()).subscribe(
      (users:any)=> this.users = users
      )
  }

  //this for register component
  postNewUser(){
    this.base.postUser(this.newUser)
  }
}
