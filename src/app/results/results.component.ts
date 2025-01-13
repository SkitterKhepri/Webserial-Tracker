import { Component, OnDestroy, OnInit } from '@angular/core';
import { FilterService } from '../services/filter.service';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { StorageService } from '../services/storage.service';

@Component({
  selector: 'app-results',
  templateUrl: './results.component.html',
  styleUrls: ['./results.component.css']
})
export class ResultsComponent implements OnInit{

  filteredSerials:any[] = []

  routerSubscription:any
  

  constructor(private filterServ:FilterService, private storage:StorageService, private router:Router){}
  

  ngOnInit(): void {
    this.filterSerials()
    this.routerSubscription = this.router.events.subscribe((event:any) => {
      if (event instanceof NavigationEnd && event.url === '/results') {
        this.refreshResults();
      }
    });
  }

  private refreshResults(): void {
    this.filterSerials()
  }

  filterSerials(updateO?:boolean , serialO?:boolean, chO?:boolean){
    const filters = this.storage.getItem("filters");
    this.filteredSerials = this.filterServ.filterSerials(filters)
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
}
