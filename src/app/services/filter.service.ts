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

  // serResults = new BehaviorSubject([])
  // chResults = new BehaviorSubject([])
  // auResults = new BehaviorSubject({})

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


  //filter:
  filterSerials(filters:any){
    let tbFiltered = this.reviewedSerials
    let filteredSerials:any = null
    if(filters.ser != undefined){
      if(filteredSerials == null){
        filteredSerials = this.serTitleFil(filters.ser, tbFiltered)
      }
      else{
        filteredSerials = this.serTitleFil(filters.ser, filteredSerials)
      }
    }
    if(filters.au != undefined){
      if(filteredSerials == null){
        filteredSerials = this.auNameFil(filters.au, tbFiltered)
      }
      else{
        filteredSerials = this.auNameFil(filters.au, filteredSerials)
      }
    }
    if(filters.status !== undefined){
      if(filteredSerials == null){
        filteredSerials = this.statusFil(filters.status, tbFiltered)
      }
      else{
        filteredSerials = this.statusFil(filters.status, filteredSerials)
      }
    }
    if(filters.chNum !== undefined){
      if(filteredSerials == null){
        filteredSerials = this.chNumFil(filters.chNum, tbFiltered)
      }
      else{
        filteredSerials = this.chNumFil(filters.chNum, filteredSerials)
      }
    }

    if(filteredSerials == null){
      return tbFiltered
    }
    else{
      return filteredSerials
    }
  }


  private statusFil(statusNum:number[], serialArray:any){
    let filteredSer:any = []
    statusNum.forEach(
      (serStat:number) => {
        filteredSer = filteredSer.concat(serialArray.filter((serial:any) => serial.status == serStat))
      }
    )
    return filteredSer
  }

  private chNumFil(serialArray:any, range?:any, from?:number, to?:number){
    let filteredSer = serialArray.filter(
      (serial:any) => {
        if(from == null && to == null){
          if(serial.chapters.length > range.start && serial.chapters.length < range.end){
            filteredSer.push(serial)
          }
          else if(from != null && to == null){
            if(serial.chapters.length > from){
              filteredSer.push(serial)
            }
          }
          else if (from == null &&to != null){
            if(serial.chapters.length < to){
              filteredSer.push(serial)
            }
          }
          else if(from != null && to != null){
            if(serial.chapters.length > from && serial.chapters.length < to){
              filteredSer.push(serial)
            }
          }
        }
      }
    )
    return filteredSer
  }

  private auNameFil(name:any, serialArray:any){
    let filteredSer = serialArray.filter(
      (serial:any) => {
        serial.author.name.toLowerCase().includes(name.toLowerCase())
      }
    )
    return filteredSer
  }

  private serTitleFila(titleSearch:any, serialArray:any){
    console.log(titleSearch)
    console.log(serialArray)
    let filteredSer:any
    serialArray.forEach((serial:any) => {
      console.log(serial)
      console.log(titleSearch)
      // if(serial.title.toLowerCase().includes(titleSearch.toLowerCase())){
      //   filteredSer.push(serial)
      // }
    });
    console.log(filteredSer)
    return filteredSer
  }

  private serTitleFil(titleSearch:any, serialArray:any[]){
    if(Array.isArray(serialArray)){
      console.log(serialArray.length)
    }
    // console.log(Array.isArray(serialArray))
    // console.log(serialArray.length)
    // console.log(serialArray)
  }

  // auSerials(auId:any){
  //   let serials = this.reviewedSerials.filter((ser:any) => ser.author.id == auId)
  //   return serials
  // }

  // emptyResults(){
  //   this.serResults.next([])
  //   this.chResults.next([])
  //   this.auResults.next({})
  // }
  
  // searchSerial(serTitle:any){
  //   let normalisedTitle = serTitle.trim().toLowerCase()
  //   let results = this.reviewedSerials.filter((ser:any) => ser.title.toLowerCase().includes(normalisedTitle))
  //   this.serResults.next(results)
  // }

  // searchChapter(chTitle:any){
  //   let normalisedTitle = chTitle.trim().toLowerCase()
  //   let results = this.reviewedSerialChapters.filter((ch:any) => ch.title.toLowerCase().includes(normalisedTitle))
  //   this.chResults.next(results)
  // }

  // searchAuthor(auName:any){
  //   let normalisedName = auName.trim().toLowerCase()
  //   let results:any[] = this.authors.filter((au:any) => au.name.toLowerCase().includes(normalisedName))
  //   let fullResults:any = []
  //   results.forEach((au:any) => {
  //     fullResults[au.name] = this.auSerials(au.id)
  //   });
  //   this.auResults.next(fullResults)
  // }
}
