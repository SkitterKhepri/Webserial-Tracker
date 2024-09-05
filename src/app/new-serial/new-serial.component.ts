import { Component } from '@angular/core';
import { AuthorsService } from '../services/authors.service';
import { SerialsService } from '../services/serials.service';
import { SerialStatuses } from '../enums/serial-statuses.enum';
import { UsersService } from '../services/users.service';

@Component({
  selector: 'app-new-serial',
  templateUrl: './new-serial.component.html',
  styleUrls: ['./new-serial.component.css']
})
export class NewSerialComponent {

  newSerial:any ={
    "id": 0,
    "title": "",
    "status": "",
    "firstCh": "",
    "home": "",
    "nextChLinkXPath": "",
    "secondaryNextChLinkXPath": "",
    "otherNextChLinkXPaths": "",
    "titleXPath": "",
    "reviewStatus": false
  }
  newSerialAuthor:any

  constructor(private serServ:SerialsService, private userServ : UsersService){}

  addSerial(authorName:any){
    if(this.userServ.getCurrentClaims().includes("Admin") || this.userServ.getCurrentClaims().includes("SAdmin")){
      this.newSerial.reviewStatus = true
    }
    this.serServ.postSerial(this.newSerial, authorName).subscribe()
    this.newSerial = {}
    this.newSerialAuthor = ""
  }
}
