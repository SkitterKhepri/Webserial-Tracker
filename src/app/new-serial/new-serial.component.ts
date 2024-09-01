import { Component } from '@angular/core';
import { AuthorsService } from '../services/authors.service';
import { SerialsService } from '../services/serials.service';

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

  constructor(private serServ:SerialsService){}

  addSerial(authorName:any, comp:any, ong:any, hia:any, aba:any){
    if (comp) {
      this.newSerial.status = "COMPLETE"
    }
    if (ong) {
      this.newSerial.status = "ONGOING"
    }
    if (hia) {
      this.newSerial.status = "HIATUS"
    }
    if (aba) {
      this.newSerial.status = "ABANDONED"
    }
    console.log(this.newSerial)
    this.serServ.postSerial(this.newSerial, authorName).subscribe()
    this.newSerial = {}
    this.newSerialAuthor = ""
  }
}
