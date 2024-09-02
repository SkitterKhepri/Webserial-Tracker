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

  constructor(private serServ:SerialsService, private authorServ:AuthorsService){
    this.getSerials()
    this.getAuthors()
  }

  getSerials(){
    this.serServ.getSerials().subscribe(
      (serials:any)=> this.serials = serials
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

  updateSerial(serial:any, authorName:any, comp:any, ong:any, hia:any, aba:any){
    if (comp) {
      serial.status = SerialStatuses.Completed
    }
    if (ong) {
      serial.status = SerialStatuses.Ongoing
    }
    if (hia) {
      serial.status = SerialStatuses.Hiatus
    }
    if (aba) {
      serial.status = SerialStatuses.Abandoned
    }
    serial.reviewStatus = true
    console.log(serial)
    return this.serServ.putSerial(serial.id, serial, authorName).subscribe(
      () => this.getSerials()
    )
  }

  deleteSerial(id:any){
    this.serServ.deleteSerial(id).subscribe(
      () => this.getSerials()
    )
  }
}
