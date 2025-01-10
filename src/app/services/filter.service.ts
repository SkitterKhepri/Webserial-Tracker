import { Injectable } from '@angular/core';
import { SerialsService } from './serials.service';
import { ChaptersService } from './chapters.service';
import { AuthorsService } from './authors.service';
import { BehaviorSubject, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FilterService {

  reviewedSerials:any = []
  reviewedSerialChapters:any = []
  reviewedSerialIds:any = []
  authors:any = []

  serResults = new BehaviorSubject([])
  chResults = new BehaviorSubject([])
  auResults = new BehaviorSubject({})

  constructor(private serServ:SerialsService) {
    this.getData()
  }

  getData(){
    this.reviewedSerials = []
    this.reviewedSerialIds = []
    this.serServ.getSerials().subscribe(
      (serials:any)=> {
        serials.forEach((serial:any) => {
          if(serial.reviewStatus){
            this.reviewedSerials.push(serial)
            this.reviewedSerialIds.push(serial.id)
            this.authors.push(serial.author)
            serial.chapters.forEach((ch:any) => {
              this.reviewedSerialChapters.push(ch)
            });
          }
        })
      }
    )
  }

  //TODO
  //filter:
  statusFil(statusNum:number[]){
    let filteredSer:any = []
    statusNum.forEach(
      (serStat:number) => {
        filteredSer = filteredSer.concat(this.reviewedSerials.filter((serial:any) => serial.status == serStat))
      }
    )
    return filteredSer
  }

  //ranges
  chNumFil(){

  }

  auNameFil(name:any){
    let filteredSer = this.reviewedSerials.filter(
      (serial:any) => {
        serial.author.name == name
      }
    )
    return filteredSer
  }

  serTitleFil(title:any){
    let filteredSer = this.reviewedSerials.filter(
      (serial:any) => {
        serial.title == title
      }
    )
    return filteredSer
  }

  //order-bys
  updateRecencyOrder(ascending:boolean){

  }

  chNumOrder(ascending:boolean){

  }

  auSerials(auId:any){
    let serials = this.reviewedSerials.filter((ser:any) => ser.author.id == auId)
    return serials
  }

  emptyResults(){
    this.serResults.next([])
    this.chResults.next([])
    this.auResults.next({})
  }
  
  searchSerial(serTitle:any){
    let normalisedTitle = serTitle.trim().toLowerCase()
    let results = this.reviewedSerials.filter((ser:any) => ser.title.toLowerCase().includes(normalisedTitle))
    this.serResults.next(results)
  }

  searchChapter(chTitle:any){
    let normalisedTitle = chTitle.trim().toLowerCase()
    let results = this.reviewedSerialChapters.filter((ch:any) => ch.title.toLowerCase().includes(normalisedTitle))
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
