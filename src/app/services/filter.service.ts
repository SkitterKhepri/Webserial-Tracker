import { Injectable } from '@angular/core';
import { SerialsService } from './serials.service';
import { ChaptersService } from './chapters.service';
import { AuthorsService } from './authors.service';
import { BehaviorSubject, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FilterService {

  constructor(private serServ:SerialsService) {}

  //filter:
  filterSerials(filters:any, tbFilteredSerials:any){
    let filteredSerials:any = null
    if(filters.ser != null){
      if(filteredSerials == null){
        filteredSerials = this.serTitleFil(filters.ser, tbFilteredSerials as Array<{}>)
      }
      else{
        filteredSerials = this.serTitleFil(filters.ser, filteredSerials as Array<{}>)
      }
    }
    if(filters.au != null){
      if(filteredSerials == null){
        filteredSerials = this.auNameFil(filters.au, tbFilteredSerials)
      }
      else{
        filteredSerials = this.auNameFil(filters.au, filteredSerials)
      }
    }
    if(filters.status != null){
      if(filteredSerials == null){
        filteredSerials = this.statusFil(filters.status, tbFilteredSerials)
      }
      else{
        filteredSerials = this.statusFil(filters.status, filteredSerials)
      }
    }
    if(filters.chNum.from != null || filters.chNum.to != null){
      if(filteredSerials == null){
        filteredSerials = this.chNumFil(tbFilteredSerials, filters.chNum.from, filters.chNum.to)
      }
      else{
        filteredSerials = this.chNumFil(filteredSerials, filters.chNum.from, filters.chNum.to)
      }
    }

    if(filteredSerials == null){
      return tbFilteredSerials
    }
    else{
      return filteredSerials
    }
  }


  private statusFil(statusNum:number[], serialArray:any){
    let filteredSer:any = []
    if(statusNum != null){
      statusNum.forEach(
        (serStat:number) => {
          filteredSer = filteredSer.concat(serialArray.filter((serial:any) => serial.status == serStat))
        }
      )
    }
    return filteredSer
  }

  private chNumFil(serialArray:any, from?:number, to?:number){
    let filteredSer:any = []
    serialArray.forEach(
      (serial:any) => {
        if(from != null && to == null){
          if(serial.chapters.length >= from){
            filteredSer.push(serial)
          }
        }
        else if (from == null && to != null){
          if(serial.chapters.length <= to){
            filteredSer.push(serial)
          }
        }
        else if(from != null && to != null){
          if(serial.chapters.length >= from && serial.chapters.length <= to){
            filteredSer.push(serial)
          }
        }
      }
    )
    return filteredSer
  }

  private auNameFil(name:any, serialArray:any){
    let filteredSer = serialArray.filter(
      (serial:any) => {
        return serial.author.name.toLowerCase().includes(name.toLowerCase())
      }
    )
    return filteredSer
  }

  private serTitleFil(titleSearch:any, serialArray:any){
    let filteredSer:any[] = []
    serialArray.forEach((serial:any) => {
      if(serial.title.toLowerCase().includes(titleSearch.toLowerCase())){
        filteredSer.push(serial)
      }
    });
    return filteredSer
  }
}
