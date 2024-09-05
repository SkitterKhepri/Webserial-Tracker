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

  reviewedSerials:any[] = []
  reviewedSerialChapters:any[] = []
  authors:any[] = []
  newChapters:any[] = []
  reviewedSerialIds:any = []

  constructor(private userServ:UsersService, private http:HttpClient, private serServ:SerialsService,
    private chServ:ChaptersService, private authorServ:AuthorsService){
    this.getSerials()
    this.getChapters()
    this.getAuthors()
  }


  getSerials(){
    this.reviewedSerials = []
    this.reviewedSerialIds = []
    this.serServ.getSerials().subscribe(
      (serials:any)=> {
        serials.forEach((serial:any) => {
          if(serial.reviewStatus){
            this.reviewedSerials.push(serial)
            this.reviewedSerialIds.push(serial.id)
          }  
        });
      }
    )
  }

  getChapters(){
    this.reviewedSerialChapters = []
    this.chServ.getChapters().subscribe(
      (chapters:any) => {
        chapters.forEach((chapter:any) => {
          if(this.reviewedSerialIds.includes(chapter.serialId)){
            this.reviewedSerialChapters.push(chapter)
          }
        });
      }
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
    return this.reviewedSerials.find((ser:any) => ser.id ==id)
  }

  getSerialChaptersCount(id:any){
    return this.reviewedSerialChapters.filter((ch:any) => ch.serialId == id).length
  }

  updateAll(){
    this.serServ.updateSerials().subscribe(
      (uCh:any) => {
        console.log(uCh + " chapters were added")
        this.getSerials()
        this.getChapters()
      }
    )
  }
}
