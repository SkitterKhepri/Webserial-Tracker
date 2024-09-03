import { Component } from '@angular/core';
import { SearchService } from '../services/search.service';
import { UsersService } from '../services/users.service';
import { StorageService } from '../services/storage.service';

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.css']
})
export class NavBarComponent {

  selectedSearch:any = "ser"

  constructor(private seaServ:SearchService,private storage:StorageService, private userServ:UsersService){}

  search(searchType:any, input:any){
    this.seaServ.emptyResults()
    switch (searchType) {
      case "ser":
        this.seaServ.searchSerial(input)
        break

      case "ch":
        this.seaServ.searchChapter(input)
        break
        
      case "au":
        this.seaServ.searchAuthor(input)
        break
        
      default:
        this.seaServ.searchSerial(input)
    }
  }

  logOut(){
    this.userServ.logOut()
  }

  getCurrentUser(){
    return this.userServ.getCurrentUser()
  }

  getCurrentUserClaims(){
    return this.userServ.getCurrentClaims()
  }
}
