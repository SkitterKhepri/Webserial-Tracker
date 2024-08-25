import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthorsService {

  constructor(private http : HttpClient) {
    this.getAuthors()
  }

  readonly apiUrl = 'https://localhost:7286/api/Authors/'

  getAuthors() : Observable<any>{
    return this.http.get(this.apiUrl)
  }

  getAuthor(id:any){
    return this.http.get(this.apiUrl + id)
  }

  postAuthor(author: any): Observable<any> {
    return this.http.post(this.apiUrl, author);
  }

  putAuthor(author:any){
    return this.http.put(this.apiUrl + author.id, author)
  }

  deleteAuthor(id:any){
    this.http.delete(this.apiUrl + id)
  }
}
