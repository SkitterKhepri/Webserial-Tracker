import { Component } from '@angular/core';
import { SerialsService } from '../services/serials.service';
import { AuthorsService } from '../services/authors.service';

@Component({
  selector: 'app-admin-serials',
  templateUrl: './admin-serials.component.html',
  styleUrls: ['./admin-serials.component.css']
})
export class AdminSerialsComponent {

  serials:any = []
  authors:any = []
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
      serial.status = "COMPLETE"
    }
    if (ong) {
      serial.status = "ONGOING"
    }
    if (hia) {
      serial.status = "HIATUS"
    }
    if (aba) {
      serial.status = "ABANDONED"
    }
    return this.serServ.putSerial(serial.id, serial, authorName).subscribe(
      () => this.getSerials()
    )
  }

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
    this.serServ.postSerial(this.newSerial, authorName).subscribe(
      () => this.getSerials()
    )
    // this.newSerial = {}
  }

  deleteSerial(id:any){
    this.serServ.deleteSerial(id).subscribe(
      () => this.getSerials()
    )
  }
}
