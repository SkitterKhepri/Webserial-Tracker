import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { FilterService } from '../services/filter.service';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { StorageService } from '../services/storage.service';
import { SerialsService } from '../services/serials.service';
import { Filters } from '../models/filters';
import { AuthService } from '../services/auth-service.service';


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

  currentFilters:any = null
  selectedChNum:any = {from : null, to : null, custom : false}
  selectedStatuses:any = []
  likedFilter:boolean = false
  newFilters:any = {ser:null, au:null}

  loggedin:boolean = false

  routerSubscription:any

  constructor(private serServ:SerialsService, private filterServ:FilterService, private storage:StorageService, private router:Router,
    private route:ActivatedRoute, private location:Location, private changer:ChangeDetectorRef, private authServ:AuthService){}

  ngOnInit(): void {
    this.authServ.loginEvent.subscribe((result:any) => this.loggedin = result)
    this.getData()
    let params = this.route.snapshot.queryParamMap
    let filters = new Filters()
    if(params.get('au')){
      filters.au = params.get('au')
      this.storage.setItem('filters', filters)
    }
    if(params.get('liked')){
      filters.liked = true
      this.storage.setItem('filters', filters)
    }
    this.routerSubscription = this.router.events.subscribe((event:any) => {
      if (event instanceof NavigationEnd && event.url === "/results") {
        this.refreshResults();
      }
    });
    this.getCurrentFilters()
    if(params.get('updateO')){
      this.updateRecencyOrder((params.get('updateO') as unknown) as boolean)
    }
    if(params.get('addedO')){
      this.updateRecencyOrder((params.get('addedO') as unknown) as boolean)
    }
    //reset url
    this.location.replaceState('/results')
  }

  ngOnDestroy(): void {
      this.clearFilters()
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
          let likedSerials = this.storage.getItem("likes")
          this.reviewedSerials.forEach((ser:any) => {
            if(likedSerials && this.authServ.getCurrentUser()){
              if(!likedSerials.includes(ser.id)){
                ser.liked = true
              }
            }
            else { ser.liked = false }
          })
        })
        this.filterSerials()
      }
    )
  }

  private refreshResults(): void {
    this.filterSerials()
    this.getCurrentFilters()
  }

  filterSerials(){
    let filters = this.storage.getItem("filters")
    this.filteredSerials = this.filterServ.filterSerials(filters, this.reviewedSerials)
  }


    
  applyNewFilters(){
    this.currentFilters.ser = this.newFilters.ser
    this.currentFilters.au = this.newFilters.au
    if(this.selectedChNum.from || this.selectedChNum.to){
      //in case somehow from is larger than to
      if(this.selectedChNum.from && this.selectedChNum.to && (this.selectedChNum.from > this.selectedChNum.to)){
        let temp = this.selectedChNum.from
        this.selectedChNum.from = this.selectedChNum.to
        this.selectedChNum.to = temp
      }
      //in case either is negative
      this.selectedChNum.from = this.selectedChNum.from < 0 ? this.selectedChNum.from = 0 : this.selectedChNum.from
      this.selectedChNum.to = this.selectedChNum.to < 0 ? this.selectedChNum.to = 0 : this.selectedChNum.to
      this.currentFilters.chNum = { ...this.selectedChNum }
    }
    this.currentFilters.status = [...this.selectedStatuses]
    this.currentFilters.liked = this.likedFilter
    this.storage.setItem("filters", this.currentFilters)
    this.filterSerials()

    this.getCurrentFilters()
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

  removeSelectedStatus(status:number){
    this.selectedStatuses = this.selectedStatuses.filter((num:number) => num !== status)
  }

  parseNum(string:string){
    return parseInt(string)
  }

  removeFilter(filterName:string){
    let filters = this.storage.getItem("filters")
    if(filterName == "chNum"){
      filters[filterName].from = null
      filters[filterName].to = null
      filters[filterName].custom = false
    }
    else if(filterName == "status"){
      filters[filterName] = []
    }
    else if(filterName == "liked"){
      filters[filterName] = false
    }
    else{
      filters[filterName] = null
    }
    this.storage.setItem("filters", filters)
    this.filterSerials()
    this.getCurrentFilters()
  }

  setChNumSelection(value:string){
    this.selectedChNum = JSON.stringify(this.selectedChNum) == value ? {from : null, to : null, custom : false} : JSON.parse(value)
  }

  clearChNumSelection(value?:any){
    console.log(this.selectedChNum)
    this.selectedChNum.from = null
    this.selectedChNum.to = null
    this.selectedChNum.custom = false
  }

  chNumFilterEquivalency(filterChNum:any){ 
    return JSON.stringify(this.selectedChNum) == filterChNum
  }

  getCurrentFilters(){
    this.currentFilters = this.storage.getItem("filters")
    this.selectedChNum = { ...this.currentFilters.chNum }
    this.selectedStatuses = [...this.currentFilters.status]
    this.likedFilter = this.currentFilters.liked
    this.changer.detectChanges()
  }

  clearFilters(){
    this.storage.setItem("filters", new Filters())
  }

  //TODO del
  debug(any:any){
    console.log(any)
  }
}
