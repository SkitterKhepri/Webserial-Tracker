import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SerialsService {
  constructor(private http : HttpClient) {
    this.getSerials()
  }

  readonly apiUrl = 'https://localhost:7286/api/Serials/'

  getSerials() : Observable<any>{
    return this.http.get(this.apiUrl)
  }

  postSerial(serial: any, authorName:any): Observable<any> {
    return this.http.post(this.apiUrl + "?authorName=" + authorName, serial);
  }

  putSerial(id:any, serial:any, authorName:any){
    return this.http.put(this.apiUrl + id + "?authorName=" + authorName, serial)
  }

  deleteSerial(id:any){
    return this.http.delete(this.apiUrl + id)
  }
}
