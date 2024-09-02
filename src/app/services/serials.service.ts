import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { StorageService } from './storage.service';

@Injectable({
  providedIn: 'root'
})
export class SerialsService {
  constructor(private http : HttpClient, private storage:StorageService) {
    this.getSerials()
  }

  readonly apiUrl = 'https://localhost:7286/'

  getSerials() : Observable<any>{
    return this.http.get(this.apiUrl + 'api/Serials/')
  }

  getSerial(id:any){
    return this.http.get(this.apiUrl + 'api/Serials/' + id)
  }

  postSerial(serial: any, authorName:any): Observable<any> {
    const token = this.storage.getItem("token")
    const headers = new HttpHeaders({"Authorization" : `Bearer ${token}`})
    console.log(serial)
    return this.http.post(this.apiUrl + 'api/Serials/' + "?authorName=" + authorName, serial, {headers});
  }

  putSerial(id:any, serial:any, authorName:any){
    return this.http.put(this.apiUrl + 'api/Serials/' + id + "?authorName=" + authorName, serial)
  }

  deleteSerial(id:any){
    return this.http.delete(this.apiUrl + 'api/Serials/' + id)
  }

  updateSerial(id:any){
    return this.http.get(this.apiUrl + "Serials/update/" + id)
  }

  updateSerials(){
    return this.http.get(this.apiUrl + "Serials/update")
  }
}
