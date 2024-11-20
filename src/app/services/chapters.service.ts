import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { StorageService } from './storage.service';

@Injectable({
  providedIn: 'root'
})
export class ChaptersService {

  constructor(private http : HttpClient, private storage:StorageService) {
    // this.getChapters()
  }

  readonly apiUrl = 'https://localhost:7286/Chapters/'

  // getChapters() : Observable<any>{
  //   return this.http.get(this.apiUrl)
  // }

  // postChapter(chapter: any): Observable<any> {
  //   const token = this.storage.getItem("token")
  //   const headers = new HttpHeaders({ "Authorization" : `Bearer ${token}`})
  //   return this.http.post(this.apiUrl, chapter, {headers});
  // }

  putChapter(chapter:any){
    const token = this.storage.getItem("token")
    const headers = new HttpHeaders({ "Authorization" : `Bearer ${token}`})
    return this.http.put(this.apiUrl + chapter.id, chapter, {headers})
  }

  deleteChapter(id:any){
    const token = this.storage.getItem("token")
    const headers = new HttpHeaders({ "Authorization" : `Bearer ${token}`})
    return this.http.delete(this.apiUrl + id, {headers})
  }
}
