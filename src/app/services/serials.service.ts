import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
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
    return this.http.get(this.apiUrl + 'api/Serials/' + id).pipe(catchError(this.handleError))
  }

  postSerial(serial: any, authorName:any): Observable<any> {
    const token = this.storage.getItem("token")
    const headers = new HttpHeaders({"Authorization" : `Bearer ${token}`})
    return this.http.post(this.apiUrl + 'api/Serials/' + "?authorName=" + authorName, serial, {headers});
  }

  putSerial(id:any, serial:any, authorName:any){
    const token = this.storage.getItem("token")
    const headers = new HttpHeaders({ "Authorization" : `Bearer ${token}`})
    return this.http.put(this.apiUrl + 'api/Serials/' + id + "?authorName=" + authorName, serial, {headers})
  }

  deleteSerial(id:any){
    const token = this.storage.getItem("token")
    const headers = new HttpHeaders({ "Authorization" : `Bearer ${token}`})
    return this.http.delete(this.apiUrl + 'api/Serials/' + id, {headers})
  }

  updateSerial(id:any){
    const token = this.storage.getItem("token")
    const headers = new HttpHeaders({ "Authorization" : `Bearer ${token}`})
    return this.http.get(this.apiUrl + "Serials/update/" + id, {headers})
  }

  updateSerials(){
    const token = this.storage.getItem("token")
    const headers = new HttpHeaders({ "Authorization" : `Bearer ${token}`})
    return this.http.get(this.apiUrl + "Serials/update", {headers})
  }

  approveSerial(id:any, reviewStatus:any){
    const token = this.storage.getItem("token")
    const headers = new HttpHeaders({ "Authorization" : `Bearer ${token}`})
    return this.http.patch(this.apiUrl + "api/Serials/" + id + "/approve", reviewStatus, {headers})
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage:any = "An unknown error occurred!";
    if (error.status === 400) {
      (typeof(error.error) == "string") ? errorMessage = error.error : errorMessage = "Bad Request: Invalid data.";
    }
    return throwError(() => new Error(errorMessage));
  }
}