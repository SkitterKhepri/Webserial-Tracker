import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { StorageService } from './storage.service';

@Injectable({
  providedIn: 'root'
})
export class AuthorsService {

  constructor(private http : HttpClient, private storage:StorageService) {
    // this.getAuthors()
  }

  readonly apiUrl = 'https://localhost:7286/api/Authors/'

  // getAuthors() : Observable<any>{
  //   return this.http.get(this.apiUrl)
  // }

  // getAuthor(id:any){
  //   return this.http.get(this.apiUrl + id)
  // }

  // postAuthor(author: any): Observable<any> {
  //   const token = this.storage.getItem("token")
  //   const headers = new HttpHeaders({ "Authorization" : `Bearer ${token}`})
  //   return this.http.post(this.apiUrl, author, {headers});
  // }

  // putAuthor(author:any){
  //   const token = this.storage.getItem("token")
  //   const headers = new HttpHeaders({ "Authorization" : `Bearer ${token}`})
  //   return this.http.put(this.apiUrl + author.id, author, {headers})
  // }

  // deleteAuthor(id:any){
  //   const token = this.storage.getItem("token")
  //   const headers = new HttpHeaders({ "Authorization" : `Bearer ${token}`})
  //   this.http.delete(this.apiUrl + id, {headers})
  // }
}
