import { Component } from '@angular/core';
import { SerialsService } from '../services/serials.service';
import { AuthorsService } from '../services/authors.service';
import { SerialStatuses } from '../enums/serial-statuses.enum';

@Component({
  selector: 'app-admin-serials',
  templateUrl: './admin-serials.component.html',
  styleUrls: ['./admin-serials.component.css']
})
export class AdminSerialsComponent {

  serials:any = []
  authors:any = []
  unReviewedSerials:any = []
  reviewedSerials:any = []

  constructor(private serServ:SerialsService, private authorServ:AuthorsService){
    this.getSerials()
    this.getAuthors()
  }

  getSerials(){
    this.reviewedSerials = []
    this.unReviewedSerials = []
    this.serServ.getSerials().subscribe(
      (serials:any)=> {
        serials.forEach((serial:any) => {
          if(serial.reviewStatus){
            this.reviewedSerials.push(serial)
          }
          if(!serial.reviewStatus){
            this.unReviewedSerials.push(serial)
          }
        });
      }
    )
  }

  getAuthors(){
    this.authorServ.getAuthors().subscribe(
      (authors:any) => this.authors = authors
    )
  }

  selectAuthor(id:any){
    return this.authors.find((au:any) => au.id == id)
  }

  updateSerial(serial:any, authorName:any){
    serial.reviewStatus = true
    return this.serServ.putSerial(serial.id, serial, authorName).subscribe(
      () => this.getSerials()
    )
  }

  deleteSerial(id:any){
    this.serServ.deleteSerial(id).subscribe(
      () => this.getSerials()
    )
  }

  approveSerial(id:any, reviewStatus:boolean){
    let reviewStatusObj = { "reviewStatus" : reviewStatus}
    this.serServ.approveSerial(id, reviewStatusObj).subscribe(
      () => this.getSerials()
    )
  }
}
