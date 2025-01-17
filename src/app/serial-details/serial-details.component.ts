import { Component } from '@angular/core';
import { SerialsService } from '../services/serials.service';
import { ActivatedRoute, Router } from '@angular/router';

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

  liked:boolean = false

  liking:boolean = false

  constructor(private serServ:SerialsService, private route:ActivatedRoute){
    this.getSerial(route.snapshot.paramMap.get("id"))
  }

  getSerial(id:any){
    //TODO delete, fix
    this.serServ.getSerials().subscribe((serial:any) => {
      this.serial = serial[id-1]
      this.pageNum = this.serial.chapters.length % this.chPerPage != 0 ? Math.floor(this.serial.chapters.length / this.chPerPage) + 1 : this.serial.chapters.length / this.chPerPage
      this.liked = this.serServ.isLiked(this.serial.id)
    })
    // this.serServ.getSerial(id).subscribe((serial:any) => {
    //   this.serial = serial
    //   this.pageNum = this.serial.chapters.length % 10 != 0 ? Math.floor(this.serial.chapters.length / 10) + 1 : this.serial.chapters.length / 10
    //   this.liked = this.serServ.isLiked(this.serial.id)
    // })
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
        this.liked = !this.liked
        this.liking = false
      }
    })
  }
}
