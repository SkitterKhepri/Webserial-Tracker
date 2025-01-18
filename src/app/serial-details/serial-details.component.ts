import { Component } from '@angular/core';
import { SerialsService } from '../services/serials.service';
import { ActivatedRoute, Router } from '@angular/router';
import { StorageService } from '../services/storage.service';

@Component({
  selector: 'app-serial-details',
  templateUrl: './serial-details.component.html',
  styleUrls: ['./serial-details.component.scss']
})
export class SerialDetailsComponent {

  
  serial:any = {
    "author" : {},
    "chapters" : [{}]
  }
  pageNum:number = 0
  currentPage:number = 1
  chPerPage:number = 10

  descExpanded:boolean = false
  longDesc:boolean = false
  descExists:boolean = false
  descDisplayLength:number = 1550

  liked:boolean = false

  liking:boolean = false

  constructor(private serServ:SerialsService, private route:ActivatedRoute, private storage:StorageService){
    this.getSerial(route.snapshot.paramMap.get("id"))
  }

  getSerial(id:any){
    // this.serServ.getSerials().subscribe((serial:any) => {
    //   this.serial = serial[id-1]
    //   this.pageNum = this.serial.chapters.length % this.chPerPage != 0 ? Math.floor(this.serial.chapters.length / this.chPerPage) + 1 : this.serial.chapters.length / this.chPerPage
    //   this.liked = this.serServ.isLiked(this.serial.id)
    // })
    this.serServ.getSerial(id).subscribe((serial:any) => {
      this.serial = serial
      this.pageNum = this.serial.chapters.length % this.chPerPage != 0 ? Math.floor(this.serial.chapters.length / this.chPerPage) + 1 : this.serial.chapters.length / this.chPerPage
      this.liked = this.serServ.isLiked(this.serial.id)
      if(serial.description != null){
        this.descExists = true
        if(serial.description.length > this.descDisplayLength){
          this.longDesc = true
        }
      }
    })
  }

  toggleDesc(){
    this.descExpanded = !this.descExpanded
  }

  nextPage(){
    this.currentPage++
  }

  prevPage(){
    this.currentPage--
  }

  jumpPage(page:number){
    this.currentPage = page
  }

  likeSerial(id:any){
    this.liking = true
    this.serServ.likeSerial(id)?.subscribe({
      error: ()=> { this.liking = false},
      complete: ()=> {
        if(!this.liked){
          let likes = this.storage.getItem("likes")
          likes.push(id)
          this.storage.setItem("likes", likes)
        }
        else{
          let likes = this.storage.getItem("likes")
          likes = likes.filter((serId:any)=> serId != id)
          this.storage.setItem("likes", likes)
        }
        this.liked = !this.liked
        this.liking = false
      }
    })
  }
}