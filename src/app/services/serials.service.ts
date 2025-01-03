import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { StorageService } from './storage.service';

@Injectable({
  providedIn: 'root'
})
export class SerialsService {
  constructor(private http : HttpClient, private storage:StorageService) {
    // this.getSerials()
  }

  readonly apiUrl = 'https://localhost:7286/Serials/'

  //TODO undo this
  getSerials() : Observable<any>{
    return this.http.get("/assets/test_serial.json")
    // return this.http.get(this.apiUrl)
  }

  getSerial(id:any){
    return this.http.get(this.apiUrl + id)
  }

  addSerial(serial: any): Observable<any> {
    const token = this.storage.getItem("token")
    const headers = new HttpHeaders({"Authorization" : `Bearer ${token}`})
    return this.http.post(this.apiUrl + "addSerial/", serial, {headers});
  }

  proposeSerial(serial: any): Observable<any> {
    const token = this.storage.getItem("token")
    const headers = new HttpHeaders({"Authorization" : `Bearer ${token}`})
    return this.http.post(this.apiUrl + "proposeSerial/", serial, {headers});
  }

  putSerial(id:any, serial:any){
    const token = this.storage.getItem("token")
    const headers = new HttpHeaders({ "Authorization" : `Bearer ${token}`})
    return this.http.put(this.apiUrl + id, serial, {headers})
  }

  deleteSerial(id:any){
    const token = this.storage.getItem("token")
    const headers = new HttpHeaders({ "Authorization" : `Bearer ${token}`})
    return this.http.delete(this.apiUrl + id, {headers})
  }

  updateSerial(id:any){
    const token = this.storage.getItem("token")
    const headers = new HttpHeaders({ "Authorization" : `Bearer ${token}`})
    return this.http.get(this.apiUrl + "update/" + id, {headers})
  }

  updateSerials(){
    const token = this.storage.getItem("token")
    const headers = new HttpHeaders({ "Authorization" : `Bearer ${token}`})
    return this.http.get(this.apiUrl + "update", {headers})
  }

  approveSerial(id:any, reviewStatus:any){
    const token = this.storage.getItem("token")
    const headers = new HttpHeaders({ "Authorization" : `Bearer ${token}`})
    return this.http.patch(this.apiUrl + id + "/approve", reviewStatus, {headers})
  }

  //Test TODO delete
  saveImage(proposedSerial:any){
    const token = this.storage.getItem("token")
    const headers = new HttpHeaders({ "Authorization" : `Bearer ${token}`})
    return this.http.post(this.apiUrl + "images/", proposedSerial, {headers})
  }

}