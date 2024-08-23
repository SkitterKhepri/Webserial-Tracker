import { Component } from '@angular/core';
import { BaseService } from '../base.service';
import { Subject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { UsersService } from '../users.service';
import { SerialsService } from '../serials.service';
import { ChaptersService } from '../chapters.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {

  users:any = []
  serials:any = []
  chapters:any = []


  constructor(private userServ:UsersService, private http:HttpClient, private serServ:SerialsService, private chServ:ChaptersService){
    this.getUsers()
    this.getSerials()
  }

  getUsers(){
    this.userServ.getUsers().subscribe(
        (users:any)=> this.users = users
      )
  }

  getSerials(){
    this.serServ.getSerials().subscribe(
      (serials:any)=> this.serials = serials
    )
  }

  getChapters(){
    this.chServ.getChapters().subscribe(
      (chapters:any) => this.chapters = chapters
    )
  }
}
