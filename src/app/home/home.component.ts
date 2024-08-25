import { Component } from '@angular/core';
import { BaseService } from '../base.service';
import { Subject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { UsersService } from '../users.service';
import { SerialsService } from '../serials.service';
import { ChaptersService } from '../chapters.service';
import { AuthorsService } from '../authors.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {

  users:any[] = []
  serials:any[] = []
  chapters:any[] = []
  authors:any[] = []
  newChapters:any[] = []

  constructor(private userServ:UsersService, private http:HttpClient, private serServ:SerialsService,
    private chServ:ChaptersService, private authorServ:AuthorsService){
    this.getUsers()
    this.getSerials()
    this.getChapters()
    this.getAuthors()
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

  getAuthors(){
    this.authorServ.getAuthors().subscribe(
      (authors:any) => this.authors = authors
    )
  }
  getAuthor(id:any) : any{
    return this.authors.find((au:any)=> au.id == id)
  }

  getSerial(id:any) :any {
    return this.serials.find((ser:any) => ser.id ==id)
  }

}
