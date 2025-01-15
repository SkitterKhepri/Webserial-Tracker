import { Component } from '@angular/core';
import { FilterService } from '../services/filter.service';
import { AuthService } from '../services/auth-service.service';
import { Router } from '@angular/router';
import { StorageService } from '../services/storage.service';
import { Filters } from '../models/filters';

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.css']
})
export class NavBarComponent {

  selectedSearch:any = "ser"
  // lastSearched:string = ""

  constructor(private filter:FilterService, private router:Router, private authServ : AuthService, private storage:StorageService){
    // this.lastSearched = storage.getItem("filters")[this.selectedSearch] ? storage.getItem("filters")[this.selectedSearch] : ""
  }

  search(searchType:any, input:any){
    let searchKey = searchType as keyof Filters
    let filters = new Filters()
    filters[searchKey] = input ? input : null
    this.storage.setItem("filters", filters)
    this.router.navigate(['/results'])
  }

  logOut(){
    this.authServ.logOut()
  }

  getCurrentUser(){
    return this.authServ.getCurrentUser()
  }

  getCurrentUserClaims(){
    return this.authServ.getCurrentClaims()
  }
}
