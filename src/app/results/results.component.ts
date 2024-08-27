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

  serials:any[] = []
  chapters:any[] = []
  authors:any[] = []

  ngOnInit(): void {
      
  }

  searchedSer:any = []
  searchedCh:any = []
  searchedAu:any = []

  authorsSerials:any = []

  constructor(private search:SearchService, private serServ:SerialsService, private chServ:ChaptersService, private authorServ:AuthorsService){
    
    this.getSerials()
    this.getChapters()
    this.getAuthors()

    this.search.serResults.subscribe(
      (res:any) => this.searchedSer = res
    )
    this.search.chResults.subscribe(
      (res:any) => this.searchedCh = res
    )
    this.search.auResults.subscribe(
      (res:any) => {
        this.searchedAu = res
        this.searchedAu.forEach((au:any) => {
          au.serials = this.auSerials(au.id)
        });
        
      }
    )
  }
  getSerials(){
    this.serServ.getSerials().subscribe(
      (serials:any)=> this.serials = serials
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

  auSerials(auId:any){
    return this.serials.filter((ser:any) => ser.authorId == auId)
  }

  getAuthor(id:any) : any{
    return this.authors.find((au:any)=> au.id == id)
  }

  getSerial(id:any) :any {
    return this.serials.find((ser:any) => ser.id ==id)
  }

  getSerialChaptersCount(id:any){
    return this.chapters.filter((ch:any) => ch.serialId == id).length
  }
}
