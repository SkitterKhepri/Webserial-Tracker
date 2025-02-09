import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { StorageService } from './storage.service';

@Injectable({
  providedIn: 'root'
})
export class ChaptersService {

  readonly apiUrl = 'https://localhost:7286/Chapters/'

  constructor(private http : HttpClient, private storage:StorageService) {}

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
