import { Component } from '@angular/core';
import { SearchService } from '../services/search.service';

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.css']
})
export class NavBarComponent {

  selectedSearch:any = "ser"

  constructor(private seaServ:SearchService){}

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
}
