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
  likedSerials:any[] = []

  loggedin:any

  //TODO maybe make it so in html not all stars start spinning, just the one clicked on? maybe.
  liking:boolean = false
  isUpdating:boolean = false
  loadingSer:boolean = false

  constructor(private serServ:SerialsService, private storage:StorageService, private authServ:AuthService){
    this.getSerials()
    authServ.loginEvent.subscribe((event:any) => this.loggedin = event)
    this.loggedin = authServ.getCurrentUser() != null
  }


  getSerials(){
    this.loadingSer = true
    this.reviewedSerials = []
    this.serServ.getSerials().subscribe({
      next: (serials:any) => {
        serials.forEach((serial:any) => {
          if(serial.reviewStatus){
            if(this.authServ.getCurrentUser()){
              let likesArr = this.storage.getItem("likes")
              if(likesArr.includes(serial.id)){
                serial.liked = true
                this.likedSerials.push(serial)
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
        this.loadingSer = false
      },
      complete: () => this.loadingSer = false
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

  likeSerial(serial:any){
    this.liking = true
    this.serServ.likeSerial(serial.id)?.subscribe({
      error: ()=> { this.liking = false},
      complete: ()=> {
        if(!serial.liked){
          this.likedSerials.push(serial)
          let likes = this.storage.getItem("likes")
          likes.push(serial.id)
          this.storage.setItem("likes", likes)
        }
        else{
          this.likedSerials = this.likedSerials.filter((ser:any)=> ser.id != serial.id)
          let likes = this.storage.getItem("likes")
          likes = likes.filter((serId:any)=> serId != serial.id)
          this.storage.setItem("likes", likes)
        }
        serial.liked = !serial.liked
        this.liking = false
      }
    })
  }
}
