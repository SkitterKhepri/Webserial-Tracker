import { Injectable } from '@angular/core';
import { SerialsService } from './serials.service';
import { ChaptersService } from './chapters.service';
import { AuthorsService } from './authors.service';
import { BehaviorSubject, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SearchService {

  serials:any = []
  chapters:any = []
  authors:any = []

  serResults = new BehaviorSubject([])
  chResults = new BehaviorSubject([])
  auResults = new BehaviorSubject({})

  constructor(private serServ:SerialsService, private chServ:ChaptersService, private auServ:AuthorsService) {

    this.getSerials()
    this.getChapters()
    this.getAuthors()

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
    this.auServ.getAuthors().subscribe(
      (authors:any) => this.authors = authors
    )
  }

  auSerials(auId:any){
    let serials = this.serials.filter((ser:any) => ser.authorId == auId)
    return serials
  }

  emptyResults(){
    this.serResults.next([])
    this.chResults.next([])
    this.auResults.next({})
  }
  
  searchSerial(serTitle:any){
    let normalisedTitle = serTitle.trim().toLowerCase()
    let results = this.serials.filter((ser:any) => ser.title.toLowerCase().includes(normalisedTitle))
    this.serResults.next(results)
  }

  searchChapter(chTitle:any){
    let normalisedTitle = chTitle.trim().toLowerCase()
    let results = this.chapters.filter((ch:any) => ch.title.toLowerCase().includes(normalisedTitle))
    this.chResults.next(results)
  }

  searchAuthor(auName:any){
    let normalisedName = auName.trim().toLowerCase()
    let results:any[] = this.authors.filter((au:any) => au.name.toLowerCase().includes(normalisedName))
    let fullResults:any = []
    results.forEach((au:any) => {
      fullResults[au.name] = this.auSerials(au.id)
    });
    this.auResults.next(fullResults)
  }
}
