import { Component } from '@angular/core';
import { AuthServiceService } from '../services/auth-service.service';
import { SerialsService } from '../services/serials.service';
import { UsersService } from '../services/users.service';

@Component({
  selector: 'app-admin-newserial',
  templateUrl: './admin-newserial.component.html',
  styleUrls: ['./admin-newserial.component.css']
})
export class AdminNewserialComponent {

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
  newSerialAuthor:any = ""
  isAdding:boolean = false
  missingData:boolean = true

  constructor(private serServ:SerialsService, private userServ : UsersService, private authServ : AuthServiceService){}

  addSerial(authorName:any){
    this.isAdding = true
    if(authorName == ""){
      authorName = "unknown"
    }
    this.serServ.postSerial(this.newSerial, authorName).subscribe({
      next: () => {
        this.isAdding = false
      },
      error: (error) => {
        console.log(error)
        this.isAdding = false
      },
      complete: () => {
        this.isAdding = false
      }
      
    })
    this.newSerial = {}
    this.newSerialAuthor = ""
  }

  isDataMissing(){
    if(this.newSerial.title == "" || this.newSerial.status == "" || this.newSerial.firstCh == "" || this.newSerial.home == "" ||
    this.newSerial.nextChLinkXPath == "" || this.newSerial.titleXPath == ""){
      this.missingData = true
    }
    else{
      this.missingData = false
    }
  }
}
