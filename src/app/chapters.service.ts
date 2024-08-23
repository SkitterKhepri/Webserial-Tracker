import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChaptersService {

  constructor(private http : HttpClient) {
    this.getChapters()
  }

  readonly apiUrl = 'https://localhost:7286/api/Chapters/'

  getChapters() : Observable<any>{
    return this.http.get(this.apiUrl)
  }

  getSerialChapters(serialId:any){
    return this.http.get(this.apiUrl + "sl/" + serialId)
  }

  postChapter(chapter: any): Observable<any> {
    return this.http.post(this.apiUrl, chapter);
  }

  putChapter(chapter:any){
    return this.http.put(this.apiUrl + chapter.id, chapter)
  }

  deleteChapter(id:any){
    this.http.delete(this.apiUrl + id)
  }
}
