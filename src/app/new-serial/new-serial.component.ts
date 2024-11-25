import { Component } from '@angular/core';
import { AuthorsService } from '../services/authors.service';
import { NavBarComponent } from '../nav-bar/nav-bar.component';
import { SerialsService } from '../services/serials.service';
import { SerialStatuses } from '../enums/serial-statuses.enum';
import { UsersService } from '../services/users.service';
import { AuthServiceService } from '../services/auth-service.service';

@Component({
  selector: 'app-new-serial',
  templateUrl: './new-serial.component.html',
  styleUrls: ['./new-serial.component.css']
})
export class NewSerialComponent {

  newSerial:any ={
    "id": 0,
    "title": "",
    "authorName": "",
    "home": "",
    "firstCh": "",
    "status": "",
    "reviewStatus": false
  }
  isProposing = false
  errorProposing = false
  constructor(private serServ:SerialsService){}

  addSerial(){
    this.isProposing = true
    this.errorProposing = false
    let formData = new FormData()
    formData.append("title", this.newSerial.title)
    formData.append("authorName", this.newSerial.authorName)
    formData.append("home", this.newSerial.home)
    formData.append("firstCh", this.newSerial.firstCh)
    formData.append("status", this.newSerial.status)
    this.serServ.proposeSerial(formData).subscribe({
      next: () => {
        this.isProposing = false
      },
      error: (error) => {
        this.errorProposing = true
        this.isProposing = false
      },
      complete: () => {
        this.isProposing = false
      }
    })
    this.newSerial = {}
  }
}
