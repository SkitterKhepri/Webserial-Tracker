import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { UsersService } from '../services/users.service';
import { SerialsService } from '../services/serials.service';
import { ChaptersService } from '../services/chapters.service';
import { AuthorsService } from '../services/authors.service';
import { Chapter, Serial } from '../models';
import { StorageService } from '../services/storage.service';
import { AuthService } from '../services/auth-service.service';

interface Banners {
  serTitle : string,
  image : any
}

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})

export class HomeComponent {

  reviewedSerials:any[] = []
  newChapters:any[] = []
  // reviewedSerialTitles:any = []
  reviewedSerialChapters:Chapter[] = []
  banners:Banners[] = [
    {
      serTitle : "placeholder",
      image : {}
    }
  ]

  loggedin:boolean = false

  //TODO maybe make it so in html not all stars start spinning, just the one clicked on? maybe.
  liking:boolean = false
  isUpdating:boolean = false

  constructor(private userServ:UsersService, private serServ:SerialsService, private storage:StorageService, private authServ:AuthService){
    this.getSerials()
    authServ.loginEvent.subscribe((event:any) => this.loggedin = event)
  }


  getSerials(){
    this.reviewedSerials = []
    this.serServ.getSerials().subscribe({
      next: (serials:any) => {
        serials.forEach((serial:any) => {
          if(serial.reviewStatus){
            let likedSerials = this.storage.getItem("likes")
            if(likedSerials && this.authServ.getCurrentUser()){
              if(!likedSerials.includes(serial.id)){
                serial.liked = true
              }
            }
            else { serial.liked = false }
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

  likeSerial(id:any){
    this.liking = true
    let serial = this.reviewedSerials.find((ser:any) => ser.id == id)
    this.serServ.likeSerial(id)?.subscribe({
      error: ()=> { this.liking = false},
      complete: ()=> {
        serial.liked = !serial.liked
        this.liking = false
      }
    })
  }
}
