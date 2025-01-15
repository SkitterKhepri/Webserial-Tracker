import { Component, OnDestroy, OnInit } from '@angular/core';
import { FilterService } from '../services/filter.service';
import { NavigationEnd, Router } from '@angular/router';
import { StorageService } from '../services/storage.service';
import { SerialsService } from '../services/serials.service';
import { Filters } from '../models/filters';
import { filter } from 'rxjs';


@Component({
  selector: 'app-results',
  templateUrl: './results.component.html',
  styleUrls: ['./results.component.css']
})
export class ResultsComponent implements OnInit, OnDestroy{

  reviewedSerials:any[] = []
  authors:any[] = []
  reviewedSerialChapters:any[] = []
  filteredSerials:any[] = []

  newFilters:Filters = new Filters()
  currentFilters:any = null
  selectedChNum?:any = null
  selectedStatuses:any = []

  routerSubscription:any
  

  constructor(private serServ:SerialsService, private filterServ:FilterService, private storage:StorageService, private router:Router){

  }


  ngOnInit(): void {
    this.currentFilters = this.storage.getItem("filters")
    this.getData()
    this.routerSubscription = this.router.events.subscribe((event:any) => {
      if (event instanceof NavigationEnd && event.url === '/results') {
        this.refreshResults();
      }
    });
    console.log(this.currentFilters)
  }

  ngOnDestroy(): void {
      //TODO need
      // this.clearFilters()
  }

  getData(){
    this.reviewedSerials = []
    this.serServ.getSerials().subscribe(
      (serials:any)=> {
        serials.forEach((serial:any) => {
          if(serial.reviewStatus){
            this.reviewedSerials.push(serial)
            this.authors.push(serial.author)
            serial.chapters.forEach((ch:any) => {
              this.reviewedSerialChapters.push(ch)
            });
          }
        })
        this.filterSerials()
      }
    )
  }

  private refreshResults(): void {
    this.currentFilters = this.storage.getItem("filters")
    this.filterSerials()
  }

  filterSerials(){
    const filters = this.storage.getItem("filters")
    this.filteredSerials = this.filterServ.filterSerials(filters, this.reviewedSerials)
    // if(updateO !== undefined){
    //   this.updateRecencyOrder(updateO)
    // }
    // if(serialO !== undefined){
    //   this.serialAddedRecencyOrder(serialO)
    // }
    // if(chO !== undefined){
    //   this.chNumOrder(chO)
    // }
    this.currentFilters = filters
  }


  applyNewFilters(){
    if(this.selectedChNum){
      this.newFilters.chNum = this.selectedChNum
    }
    this.newFilters.status = this.selectedStatuses
    this.storage.setItem("filters", this.newFilters)
    this.filterSerials()
  }



  //order-bys
  updateRecencyOrder(ascending:boolean){
    this.filteredSerials.sort((a:any, b:any) =>{
      let maxX = Math.max(...a.chapters.map((chapter:any) => chapter.id));
      let maxY = Math.max(...b.chapters.map((chapter:any) => chapter.id));
      return ascending ? maxX - maxY : maxY - maxX
    })
  }

  serialAddedRecencyOrder(ascending:boolean){
    this.filteredSerials.sort((a:any, b:any) =>{
      return ascending ? a.id - b.id : b.id - a.id
    })
  }

  chNumOrder(ascending:boolean){
    this.filteredSerials.sort((a:any, b:any) =>{
      let aNum = a.chapters.length
      let bNum = b.chapters.length
      return ascending ? aNum - bNum : bNum - aNum
    })
  }

  areThereFilters(currFil:any){
    return JSON.stringify(currFil) != JSON.stringify(new Filters())
  }

  statusFilterIncludes(statusValue:string){
    return this.currentFilters.status.includes(parseInt(statusValue))
  }

  parseNum(string:string){
    return parseInt(string)
  }

  removeFilter(filterName:string){
    let filters = this.storage.getItem("filters")
    filterName != "chNum" ? filters[filterName] = null : ( filters[filterName].from = null, filters[filterName].to = null)
    this.storage.setItem("filters", filters)
    this.filterSerials()
  }

  clearChNumSelection(value:any){
    if(value){
      this.selectedChNum = null
    }
  }

  clearFilters(){
    this.storage.setItem("filters", new Filters())
  }
}
