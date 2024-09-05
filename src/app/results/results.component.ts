import { Component, OnInit } from '@angular/core';
import { SearchService } from '../services/search.service';
import { SerialsService } from '../services/serials.service';
import { ChaptersService } from '../services/chapters.service';
import { AuthorsService } from '../services/authors.service';

@Component({
  selector: 'app-results',
  templateUrl: './results.component.html',
  styleUrls: ['./results.component.css']
})
export class ResultsComponent implements OnInit {

  reviewedSerials:any[] = []
  chapters:any[] = []
  authors:any[] = []

  ngOnInit(): void {
  }

  searchedSer:any
  searchedCh:any
  searchedAu: { [key: string]: any[] } = {}


  constructor(private search:SearchService, private serServ:SerialsService, private chServ:ChaptersService, private authorServ:AuthorsService){
    
    this.getSerials()
    this.getChapters()
    this.getAuthors()
    
    this.serResults()
    this.chResults()
    this.auResults()
  }
  getSerials(){
    this.serServ.getSerials().subscribe(
      (serials:any)=> {
        serials.forEach((serial:any) => {
          if(serial.reviewStatus){
            this.reviewedSerials.push(serial)
          }  
        });
      }
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
    return this.reviewedSerials.find((ser:any) => ser.id ==id)
  }

  getSerialChaptersCount(id:any){
    return this.chapters.filter((ch:any) => ch.serialId == id).length
  }

  serResults(){
    this.search.serResults.subscribe(
      (res:any) => this.searchedSer = res
    )
  }

  chResults(){
    this.search.chResults.subscribe(
      (res:any) => this.searchedCh = res
    )
  }

  auResults(){
    this.search.auResults.subscribe(
      (res:any) => this.searchedAu = res
    )
  }
}
