import { Component, OnDestroy, OnInit } from '@angular/core';
import { FilterService } from '../services/filter.service';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { StorageService } from '../services/storage.service';
import { SerialsService } from '../services/serials.service';

@Component({
  selector: 'app-results',
  templateUrl: './results.component.html',
  styleUrls: ['./results.component.css']
})
export class ResultsComponent implements OnInit{

  reviewedSerials:any[] = []
  authors:any[] = []
  reviewedSerialChapters:any[] = []
  filteredSerials:any[] = []

  currentFilters:any = null

  routerSubscription:any
  

  constructor(private serServ:SerialsService, private filterServ:FilterService, private storage:StorageService, private router:Router){

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

    this.currentFilters.au = "Wildbow"
    this.currentFilters.chNum = {}
    this.currentFilters.chNum.from = "100"
    this.currentFilters.chNum.to = "200"
    this.currentFilters.status = "1"

  }


  ngOnInit(): void {
    this.currentFilters = this.storage.getItem("filters")
    this.getData()
    this.routerSubscription = this.router.events.subscribe((event:any) => {
      if (event instanceof NavigationEnd && event.url === '/results') {
        this.refreshResults();
      }
    });
  }

  private refreshResults(): void {
    this.currentFilters = this.storage.getItem("filters")
    this.filterSerials()
  }

  filterSerials(updateO?:boolean , serialO?:boolean, chO?:boolean){
    const filters = this.storage.getItem("filters");
    this.filteredSerials = this.filterServ.filterSerials(filters, this.reviewedSerials)
    if(updateO !== undefined){
      this.updateRecencyOrder(updateO)
    }
    if(serialO !== undefined){
      this.serialAddedRecencyOrder(serialO)
    }
    if(chO !== undefined){
      this.chNumOrder(chO)
    }
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
    return currFil == "{\"ser\":\"\"}"
  }
}
