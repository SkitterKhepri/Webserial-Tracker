import { Component } from '@angular/core';
import { BaseService } from '../services/base.service';
import { Subject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { UsersService } from '../services/users.service';
import { SerialsService } from '../services/serials.service';
import { ChaptersService } from '../services/chapters.service';
import { AuthorsService } from '../services/authors.service';
import { Author, Chapter, Serial } from '../models';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {

  reviewedSerials:Serial[] = []
  authors:Author[] = []
  newChapters:any[] = []
  reviewedSerialIds:any = []
  reviewedSerialChapters:Chapter[] = []

  isUpdating:boolean = false

  constructor(private userServ:UsersService, private http:HttpClient, private serServ:SerialsService,
    private chServ:ChaptersService, private authorServ:AuthorsService){
    this.getSerials()
    // this.getChapters()
    // this.getAuthors()
  }


  getSerials(){
    this.reviewedSerials = []
    this.reviewedSerialIds = []
    this.serServ.getSerials().subscribe({
      next: (serials:any) => {
        serials.forEach((serial:any) => {
          if(serial.reviewStatus){
            this.reviewedSerials.push(serial)
            this.reviewedSerialChapters.push(serial.chapters)
          }  
        });
      },
      error: (response:any) => {
        console.log("shits fucked: " + response)
      }
    })
  }

  // getChapters(){
  //   this.reviewedSerialChapters = []
  //   this.chServ.getChapters().subscribe(
  //     (chapters:any) => {
  //       chapters.forEach((chapter:any) => {
  //         if(this.reviewedSerialIds.includes(chapter.serialId)){
  //           this.reviewedSerialChapters.push(chapter)
  //         }
  //       });
  //     }
  //   )
  // }

  getSerial(id:any){
    return this.reviewedSerials.find((ser:any) => ser.id == id)
  }

  getSerialChaptersCount(id:any){
    let serial = this.getSerial(id)
    if (serial != undefined){
      return serial.chapters.length
    }
    else{
      return null
    }
  }

  updateAll(){
    this.isUpdating = true
    this.serServ.updateSerials().subscribe({
      next: (uCh:any) => {
        console.log(uCh + " chapters were added")
        this.getSerials()
        // this.getChapters()
        this.isUpdating = false
      },
      error: (response) => {
        console.log("updating shat itself: " + response)
        this.isUpdating = false
      },
      complete: () => {
        this.isUpdating = false
      }
    })
  }
}
