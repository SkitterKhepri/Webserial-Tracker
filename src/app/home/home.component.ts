import { Component } from '@angular/core';
import { BaseService } from '../base.service';
import { Subject } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {

  users:any = []


  constructor(private base:BaseService, private http:HttpClient){
    this.getUsers()
  }

  async kiir(){
    (await this.base.getUsers()).subscribe(
      (us:any) => {
        this.users = us
      }
    )

    console.log(this.users)
  }

  async getUsers(){
    (await this.base.getUsers()).subscribe(
      (users:any)=> this.users = users
      )
  }

  kiirUsers(){
    console.log(this.users)
  }

}
