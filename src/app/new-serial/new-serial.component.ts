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
    "reviewStatus": true
  }
  newSerialAuthor:any

  constructor(private serServ:SerialsService, private userServ : UsersService){}

  addSerial(authorName:any, comp:any, ong:any, hia:any, aba:any){
    if (comp) {
      this.newSerial.status = SerialStatuses.Completed
    }
    if (ong) {
      this.newSerial.status = SerialStatuses.Ongoing
    }
    if (hia) {
      this.newSerial.status = SerialStatuses.Hiatus
    }
    if (aba) {
      this.newSerial.status = SerialStatuses.Abandoned
    }
    if(this.userServ.getCurrentClaims().includes("Admin") || this.userServ.getCurrentClaims().includes("SAdmin")){
      this.newSerial.reviewStatus = true
    }
    console.log(this.newSerial)
    this.serServ.postSerial(this.newSerial, authorName).subscribe()
    this.newSerial = {}
    this.newSerialAuthor = ""
  }
}
