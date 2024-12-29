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
  }


  getSerials(){
    this.reviewedSerials = []
    this.reviewedSerialIds = []
    this.serServ.getSerials().subscribe({
      next: (serials:any) => {
        serials.forEach((serial:any) => {
          if(serial.reviewStatus){
            this.reviewedSerials.push(serial)
            serial.chapters.forEach((ch:any) => {
              this.reviewedSerialChapters.push(ch)
            });
          }  
        });
      },
      error: (response:any) => {
        console.log("shits fucked: " + response)
      }
    })
  }

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
        // this.isUpdating = false
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
