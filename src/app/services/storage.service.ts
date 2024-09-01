import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class StorageService {

  constructor() { }

  setItem(key:string, value:any){
    localStorage.setItem(key, JSON.stringify(value))
  }

  // getItem(key: string) {
  //   return localStorage.getItem(key);
  // }

  getItem(key:any) {
    const stored = localStorage.getItem(key)
    return stored ? JSON.parse(stored) : null
  }

  clear() {
    localStorage.clear();
  }

}
