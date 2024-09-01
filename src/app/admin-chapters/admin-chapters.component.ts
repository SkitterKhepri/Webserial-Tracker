import { Component } from '@angular/core';
import { ChaptersService } from '../services/chapters.service';
import { SerialsService } from '../services/serials.service';

@Component({
  selector: 'app-admin-chapters',
  templateUrl: './admin-chapters.component.html',
  styleUrls: ['./admin-chapters.component.css']
})
export class AdminChaptersComponent {

  chapters:any[] = []
  serials:any[] = []

  constructor(private chServ:ChaptersService, private serServ:SerialsService){
    this.getChapters()
    this.getSerials()
  }

  getChapters(){
    this.chServ.getChapters().subscribe(
      (chapters:any) => this.chapters = chapters
    )
  }

  deleteChapter(id:number){
    this.chServ.deleteChapter(id).subscribe(
      () => this.getChapters()
    )
  }
  
  updateChapter(chapter:any){
    this.chServ.putChapter(chapter).subscribe(
      () => this.getChapters()
    )
  }

  getSerials(){
    this.serServ.getSerials().subscribe(
      (serials:any)=> this.serials = serials
    )
  }

  selectSerial(id:any){
    return this.serials.find((ser:any) => ser.id == id)
  }
}
