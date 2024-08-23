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

  postSerial(serial: any): Observable<any> {
    return this.http.post(this.apiUrl, serial);
  }
  

  putSerial(serial:any){
    return this.http.put(this.apiUrl + serial.id, serial)
  }

  deleteSerial(id:any){
    this.http.delete(this.apiUrl + id)
  }
}
