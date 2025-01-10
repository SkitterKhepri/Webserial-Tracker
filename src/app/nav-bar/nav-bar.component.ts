import { Component } from '@angular/core';
import { FilterService } from '../services/filter.service';
import { UsersService } from '../services/users.service';
import { StorageService } from '../services/storage.service';
import { AuthService } from '../services/auth-service.service';

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.css']
})
export class NavBarComponent {

  selectedSearch:any = "ser"

  constructor(private filter:FilterService,private storage:StorageService, private userServ:UsersService, private authServ : AuthService){}

  search(searchType:any, input:any){
    this.filter.emptyResults()
    switch (searchType) {
      case "ser":
        this.filter.searchSerial(input)
        break

      case "ch":
        this.filter.searchChapter(input)
        break
        
      case "au":
        this.filter.searchAuthor(input)
        break
        
      default:
        this.filter.searchSerial(input)
    }
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
