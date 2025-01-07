import { Component } from '@angular/core';
import { BaseService } from '../services/base.service';
import { Subject } from 'rxjs';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { UsersService } from '../services/users.service';
import { SerialsService } from '../services/serials.service';
import { ChaptersService } from '../services/chapters.service';
import { AuthorsService } from '../services/authors.service';
import { Author, Chapter, Serial } from '../models';

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

  reviewedSerials:Serial[] = []
  newChapters:any[] = []
  // reviewedSerialTitles:any = []
  reviewedSerialChapters:Chapter[] = []
  banners:Banners[] = [
    {
      serTitle : "placeholder",
      image : {}
    }
  ]

  isUpdating:boolean = false

  constructor(private userServ:UsersService, private http:HttpClient, private serServ:SerialsService,
    private chServ:ChaptersService, private authorServ:AuthorsService){
    this.getSerials()
  }


  getSerials(){
    this.reviewedSerials = []
    // this.reviewedSerialTitles = []
    this.serServ.getSerials().subscribe({
      next: (serials:any) => {
        serials.forEach((serial:any) => {
          if(serial.reviewStatus){
            // this.reviewedSerialTitles.push(serial.title)
            this.reviewedSerials.push(serial)
            serial.chapters.forEach((ch:any) => {
              this.reviewedSerialChapters.push(ch)
            });
          }  
        });
        // this.getImages(this.reviewedSerialTitles)
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

  // getImages(serialTitles:any){
  //   serialTitles.forEach((serialTitle:string) => {
  //     this.serServ.getImage(serialTitle).subscribe({
  //       next: (response:HttpResponse<Blob>) => {
  //         let imgBlob = response.body
  //         this.banners.push({serTitle : serialTitle, image : imgBlob})
  //       },
  //       error: () => {}
  //     })
  //   });
  // }

  // displayImg(serialTitle:string){
  //   let url:any
  //   try {
  //     url = URL.createObjectURL(this.banners.find((banner:any) => banner.serTitle == serialTitle)?.image)
  //   } catch (error) {
      
  //   }
  //   return url
  // }

  // normalizeSerTit(serialTitle:string){
  //   return serialTitle.trim().replaceAll(' ', '_').replaceAll(':', '_').replaceAll('-', '_').toLowerCase()
  // }
}
