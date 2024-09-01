import { Component } from '@angular/core';
import { BaseService } from '../services/base.service';
import { Subject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { UsersService } from '../services/users.service';
import { SerialsService } from '../services/serials.service';
import { ChaptersService } from '../services/chapters.service';
import { AuthorsService } from '../services/authors.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {

  serials:any[] = []
  chapters:any[] = []
  authors:any[] = []
  newChapters:any[] = []
  currentUser:any = null
  currentUserClaims:any = null

  constructor(private userServ:UsersService, private http:HttpClient, private serServ:SerialsService,
    private chServ:ChaptersService, private authorServ:AuthorsService){
    this.getSerials()
    this.getChapters()
    this.getAuthors()
    this.getCurrentUser()
    this.getCurrentUserClaims()
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

  getSerialChaptersCount(id:any){
    return this.chapters.filter((ch:any) => ch.serialId == id).length
  }

  updateAll(){
    this.serServ.updateSerials().subscribe(
      (ser:any) => {
        this.serials = ser
        this.getChapters()
        this.getAuthors()
      }
    )
  }

  getCurrentUser(){
    this.userServ.currentUser.subscribe(
      (us:any) => this.currentUser = us
    )
  }

  getCurrentUserClaims(){
    this.userServ.currentUserClaims.subscribe(
      (cl:any) => this.currentUserClaims = cl
    )
  }
}
