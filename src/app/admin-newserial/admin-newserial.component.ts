import { Component } from '@angular/core';
import { AuthServiceService } from '../services/auth-service.service';
import { SerialsService } from '../services/serials.service';
import { UsersService } from '../services/users.service';
import { Serial } from '../models';

@Component({
  selector: 'app-admin-newserial',
  templateUrl: './admin-newserial.component.html',
  styleUrls: ['./admin-newserial.component.css']
})
export class AdminNewserialComponent {

  newSerial:any = {}
  newSerialAuthor:any = ""
  isAdding:boolean = false
  missingData:boolean = true
  errorAdding:boolean = false

  constructor(private serServ:SerialsService, private userServ : UsersService, private authServ : AuthServiceService){}

  addSerial(authorName:any){
    this.isAdding = true
    let formData = new FormData()
    if(authorName == ""){
      authorName = "unknown"
    }
    this.serServ.addSerial(formData).subscribe({
      next: () => {
        this.isAdding = false
      },
      error: (error) => {
        this.errorAdding = true
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
